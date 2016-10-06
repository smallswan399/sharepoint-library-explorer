using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Core;
using Core.Libs;
using Entities;
using Main.Services.Domains;
using Microsoft.SharePoint.Client;
using Services;

namespace Main.Services
{
    public class SharePointService : ISharePointService
    {
        private readonly ClientContext _context;
        public ClearTextCredential Credential { get; private set; }
        public SharePointServerVersion SharePointServerVersion { get; private set; }
        public SiteSoapClientMapper SiteSoapClientMapper { get; private set; }
        public SharePointService
            (ClientContext context,
            ClearTextCredential credential,
            SiteSoapClientMapper siteSoapClientMapper,
            SharePointServerVersion sharePointServerVersion)
        {
            SiteSoapClientMapper = siteSoapClientMapper;
            this._context = context;
            SharePointServerVersion = sharePointServerVersion;
            context.Credentials = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            Credential = credential;
        }

        public TryConnectResult TestConnection()
        {
            try
            {
                var web = _context.Web;
                _context.Load(web);
                _context.ExecuteQuery();
                return new TryConnectResult
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return new TryConnectResult()
                {
                    Message = ex.Message
                };
            }
        }

        public SharePointSite GetSharePointSite()
        {
            try
            {
                var site = _context.Web;
                _context.Load(site);
                _context.Load(site, s => s.CurrentUser.LoginName);
                
                _context.ExecuteQuery();
                return new SharePointSite()
                {
                    RelativeUrl = site.ServerRelativeUrl,
                    Id = site.Id,
                    Url = site.Url,
                    //RootUrl = Utils.GetRootUrl(site.Url, site.ServerRelativeUrl),
                    Title = site.Title,
                    User =
                        site.CurrentUser.LoginName.Contains("|")
                            ? site.CurrentUser.LoginName.Split('|')[1]
                            : site.CurrentUser.LoginName,
                    Description = site.Description
                };
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<SharePointSite> GetSharePointSiteAsync()
        {
            return await Task.Factory.StartNew(() => GetSharePointSite());
        }

        /// <summary>
        /// Get sub sites of current site
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SharePointSite> GetSubSharePointSites()
        {
            try
            {
                var subSites = _context.Web.Webs;
                _context.Load(subSites, s => s.Include(t => t.Url), s => s.Include(t => t.Description),
                    s => s.Include(t => t.Title), s => s.Include(t => t.ServerRelativeUrl));
                _context.ExecuteQuery();
                return subSites.ToList().Select(s => new SharePointSite()
                {
                    Url = s.Url,
                    Description = s.Description,
                    Title = s.Title,
                    RelativeUrl = s.ServerRelativeUrl
                });
            }
            catch (Exception ex)
            {
                return new List<SharePointSite>();
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<IEnumerable<SharePointSite>> GetSubSharePointSitesAsync()
        {
            return await Task.Factory.StartNew(() => GetSubSharePointSites());
        }

        /// <summary>
        /// Get Libraries of current site
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SharePointLibrary> GetSharePointLibraries()
        {
            try
            {
                var lists = _context.Web.Lists;
                _context.Load(lists, s => s.Include(t => t.RootFolder.ServerRelativeUrl), s => s.Include(t => t.Hidden),
                    s => s.Include(t => t.Description), s => s.Include(t => t.Title),
                    s => s.Include(t => t.BaseTemplate),
                    s => s.Include(t => t.EnableVersioning),
                    s => s.Include(t => t.ForceCheckout));
                _context.ExecuteQuery();
                var result = lists.ToList()
                    .Where(s => !s.Hidden && ((ListTemplateType)s.BaseTemplate == ListTemplateType.DocumentLibrary))
                    .Select(s => new SharePointLibrary()
                    {
                        RelativeUrl = s.RootFolder.ServerRelativeUrl,
                        Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.RootFolder.ServerRelativeUrl),
                        Description = s.Description,
                        Title = s.Title,
                        EnableVersioning = s.EnableVersioning,
                        RequireCheckout = s.ForceCheckout
                    });
                return result;
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<IEnumerable<SharePointLibrary>> GetSharePointLibrariesAsync()
        {
            return await Task.Factory.StartNew(() => GetSharePointLibraries());
        }

        /// <summary>
        /// Not need Implement for SharePoint 2013
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public SharePointLibrary GetSharePointLibrary(string title)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get Folders of a Library by Library title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public IEnumerable<SharePointFolder> GetSharePointFolders(string title)
        {
            try
            {
                var list = _context.Web.Lists.GetByTitle(title);
                var folders = list.GetItems(new CamlQuery()
                {
                    ViewXml = "<View><Query><Where><Eq><FieldRef Name='ContentType' /><Value Type='Computed'>Folder</Value></Eq></Where></Query></View>"
                });
                _context.Load(folders, s => s.Include(t => t.DisplayName),
                    s => s.Include(t => t.Folder.Properties),
                    s => s.Include(t => t.Folder.ServerRelativeUrl));
                _context.ExecuteQuery();
                return folders.ToList().Select(s => new SharePointFolder()
                {
                    Title = s.DisplayName,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.Folder.ServerRelativeUrl),
                    RelativeUrl = s.Folder.ServerRelativeUrl,
                    Name = s.DisplayName,
                    CreatedDateTime = (DateTime)s.Folder.Properties["vti_timecreated"],
                    LastModifiedDateTime = (DateTime)s.Folder.Properties["vti_timelastmodified"],
                });
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<IEnumerable<SharePointFolder>> GetSharePointFolderAsync(string title)
        {
            return await Task.Factory.StartNew(() => GetSharePointFolders(title));
        }

        public IEnumerable<SharePointFolder> GetSharePointSubFolder(string relativeUrl)
        {
            try
            {
                var parentFolder = _context.Web.GetFolderByServerRelativeUrl(relativeUrl);
                var folders = parentFolder.Folders;
                _context.Load(folders, s => s.Include(t => t.ServerRelativeUrl), s => s.Include(t => t.Name));
                _context.ExecuteQuery();
                return folders.ToList().Select(s => new SharePointFolder()
                {
                    Title = s.Name,
                    Name = s.Name,
                    RelativeUrl = s.ServerRelativeUrl,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ServerRelativeUrl)
                });
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<IEnumerable<SharePointFolder>> GetSharePointSubFolderAsync(string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetSharePointSubFolder(relativeUrl));
        }

        /// <summary>
        /// Get file of a Library
        /// </summary>
        /// <param name="listTitle">Library title</param>
        /// <returns></returns>
        public IEnumerable<ISharePointFile> GetSharePointFiles(string listTitle)
        {
            try
            {
                var lists = _context.Web.Lists;
                var list = lists.GetByTitle(listTitle);
                var result = new List<ISharePointFile>();
                var files = list.RootFolder.Files;

                _context.Load(files, s => s.Include(t => t.Author), s => s.Include(t => t.ServerRelativeUrl),
                    s => s.Include(t => t.Name), s => s.Include(t => t.TimeCreated), s => s.Include(t => t.TimeLastModified),
                    s => s.Include(t => t.Length), s => s.Include(t => t.ListItemAllFields), s => s.Include(t => t.CheckedOutByUser));
                _context.Load(files, s => s.Include(t => t.UIVersionLabel));

                _context.ExecuteQuery();
                files.ToList().ForEach(s =>
                {
                    var newFile = new SharePointFile
                    {
                        RelativeUrl = s.ServerRelativeUrl,
                        Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ServerRelativeUrl),
                        Name = s.Name,
                        CreatedDateTime = (DateTime)s.ListItemAllFields.FieldValues["Created"],
                        LastModifiedDateTime = (DateTime)s.ListItemAllFields.FieldValues["Modified"],
                        Author = s.Author == null ? "" : (s.Author.LoginName.Contains("|") ? s.Author.LoginName.Split('|')[1] : s.Author.LoginName),
                        FileSize = s.Length,
                        FileExtension =
                            s.ListItemAllFields.FieldValues["File_x0020_Type"] == null
                                ? ""
                                : s.ListItemAllFields.FieldValues["File_x0020_Type"].ToString(),
                        CheckoutUser = GetCheckOutUserName(s.CheckedOutByUser),
                        UIVersionLabel = s.UIVersionLabel
                    };
                    result.Add(newFile);
                });
                return result;
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<IEnumerable<ISharePointFile>> GetSharePointFilesAsync(string listTitle)
        {
            return await Task.Factory.StartNew(() => GetSharePointFiles(listTitle));
        }

        /// <summary>
        /// Get file of a Library
        /// </summary>
        /// <param name="relativeUrl">Library relativeUrl</param>
        /// <returns></returns>
        public IEnumerable<ISharePointFile> GetSharePointFilesInFolder(string relativeUrl)
        {
            try
            {
                var folder = _context.Web.GetFolderByServerRelativeUrl(relativeUrl);
                var files = folder.Files;
                _context.Load(files, s => s.Include(t => t.ListItemAllFields), s => s.Include(t => t.Author));
                _context.ExecuteQuery();

                return files.ToList().Select(s => new SharePointFile()
                {
                    RelativeUrl = s.ListItemAllFields["FileRef"].ToString(),
                    Name = s.ListItemAllFields["FileLeafRef"].ToString(),
                    CreatedDateTime = (DateTime)s.ListItemAllFields["Created"],
                    LastModifiedDateTime = (DateTime)s.ListItemAllFields["Modified"],
                    FileSize = long.Parse(s.ListItemAllFields["File_x0020_Size"].ToString()),
                    Author =
                        s.Author == null
                            ? ""
                            : (s.Author.LoginName.Contains("|") ? s.Author.LoginName.Split('|')[1] : s.Author.LoginName),
                    FileExtension =
                        s.ListItemAllFields["File_x0020_Type"] == null
                            ? ""
                            : s.ListItemAllFields["File_x0020_Type"].ToString(),
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ListItemAllFields["FileRef"].ToString()),
                    UIVersionLabel = s.ListItemAllFields["_UIVersionString"].ToString(),
                    CheckoutUser = GetCheckOutUserName(s.CheckedOutByUser)
                });
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<IEnumerable<ISharePointFile>> GetSharePointFilesInFolderAsync(string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetSharePointFilesInFolder(relativeUrl));
        }

        /// <summary>
        /// Download file by using absoluteUrl
        /// </summary>
        /// <param name="url">absolute Url</param>
        /// <returns></returns>
        public Stream DownloadFileByUrl(string url)
        {
            //try
            //{
            //    var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);

            //    var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
            //    context.Load(file, s => s.Name, s => s.ServerRelativeUrl, s => s.Versions, s => s.UIVersionLabel, s => s.UIVersion);
            //    context.ExecuteQuery();
            //    var finfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, relativeUrl);
            //    return finfo.Stream;
            //}
            //catch (Exception ex)
            //{
            //    // try/catch for just debug only, dont handle error here
            //    throw;
            //}

            var absoluteUrl = url;
            var request = (HttpWebRequest)WebRequest.Create(absoluteUrl);

            request.Credentials = new NetworkCredential(Credential.Username, Credential.Password, Credential.DomainName);
            request.Timeout = 10000;
            request.AllowWriteStreamBuffering = false;
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }

        public async Task<Stream> DownloadFileByUrlAsync(string url)
        {
            return await Task.Factory.StartNew(() => DownloadFileByUrl(url));
        }

        public List<CopyActionResult> UploadFile(string parentRelativeUrl, string name, Stream streamFile)
        {
            if (_context.HasPendingRequest)
            _context.ExecuteQuery();
            try
            {
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(_context, parentRelativeUrl + "/" + name, streamFile, true);
                return new List<CopyActionResult>(){new CopyActionResult()
                {
                    ErrorCode = ErrorCode.Success,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(parentRelativeUrl, name)
                }};
            }
            catch (Exception ex)
            {
                return new List<CopyActionResult>() { new CopyActionResult() { ErrorCode = ErrorCode.Unknown, Message = ex.Message } };
            }
        }

        public async Task<List<CopyActionResult>> UploadFileAsync(string parentRelativeUrl, string name, Stream streamFile)
        {
            return await Task.Factory.StartNew(() => UploadFile(parentRelativeUrl, name, streamFile));
        }

        public List<CopyActionResult> UploadFile(string absoluteUrl, Stream streamFile)
        {
            var relativeUrl = absoluteUrl.Replace(SiteSoapClientMapper.RootUrl, string.Empty);
            if (_context.HasPendingRequest)
            _context.ExecuteQuery();
            try
            {

                Microsoft.SharePoint.Client.File.SaveBinaryDirect(_context, relativeUrl, streamFile, true);
                // MessageBox.Show("upload success");
                return new List<CopyActionResult>(){new CopyActionResult()
                {
                    ErrorCode = ErrorCode.Success,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(absoluteUrl)
                }};
            }
            catch (Exception ex)
            {
                // MessageBox.Show("upload error");
                return new List<CopyActionResult>() { new CopyActionResult() { ErrorCode = ErrorCode.Unknown, Message = ex.Message } };
            }
        }


        public bool CheckFileExist(string parentRelativeUrl, string name)
        {
            try
            {
                var parentFolder = _context.Web.GetFolderByServerRelativeUrl(parentRelativeUrl);
                var files = parentFolder.Files;
                var exFiles = _context.LoadQuery(files.Where(s => s.Name == name));
                _context.ExecuteQuery();
                return exFiles.Any();
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        //public void RefreshCredential()
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<SharePointFileVersion> GetVersions(string url, string relativeUrl)
        {
            try
            {
                var file = _context.Web.GetFileByServerRelativeUrl(relativeUrl);

                _context.Load(file, s => s.Name, s => s.ServerRelativeUrl, s => s.ListItemAllFields, s => s.TimeCreated,
                    s =>
                        s.Versions.Include(t => t.Created, t => t.Url, t => t.CheckInComment, t => t.CreatedBy, t => t.ID,
                            t => t.ID, t => t.Size, t => t.VersionLabel), s => s.UIVersionLabel, s => s.UIVersion,
                    s => s.ModifiedBy, s => s.TimeLastModified, s => s.Length);
                
                _context.ExecuteQuery();

                var timeZonoOffset = (DateTime)file.ListItemAllFields.FieldValues["Created"] - file.TimeCreated;

                var result = file.Versions.ToList().Select(s => new SharePointFileVersion()
                {
                    Url = SiteSoapClientMapper.Url.CombineUrl(s.Url),
                    RelativeUrl = s.Url,
                    CheckInComment = s.CheckInComment,
                    Created = s.Created + timeZonoOffset,
                    CreatedBy = s.CreatedBy.LoginName.Split(new[] { "\\" }, StringSplitOptions.None)[1],
                    ID = s.ID,
                    Size = s.Size,
                    VersionLabel = s.VersionLabel,
                    // Url = Utils.GetRootUrl(url, relativeUrl) + s.Url
                }).ToList();

                var currentVersion = new SharePointFileVersion
                {
                    Url = url,
                    RelativeUrl = relativeUrl,
                    Created = (DateTime)file.ListItemAllFields.FieldValues["Modified"],
                    CreatedBy = file.ModifiedBy.LoginName.Split(new[] { "\\" }, StringSplitOptions.None)[1],
                    Size = file.Length,
                    CheckInComment = "",
                    OriginalFileName = file.Name,
                    VersionLabel = file.UIVersionLabel
                };
                result.Add(currentVersion);

                return result;
            }
            catch (Exception ex)
            {
                // try/catch for just debug only, dont handle error here
                //return new List<SharePointFileVersion>();
                //throw ex;
                throw;
            }
        }

        public async Task<IEnumerable<SharePointFileVersion>> GetVersionsAsync(string url, string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetVersions(url, relativeUrl));
        }

        public Stream DownloadHistoryVersion(string url)
        {
            // This method can download both history version and current version. This will download the file by using file url
            var absoluteUrl = url;
            var request = (HttpWebRequest)WebRequest.Create(absoluteUrl);

            request.Credentials = new NetworkCredential(Credential.Username, Credential.Password, Credential.DomainName);
            request.Timeout = 10000;
            request.AllowWriteStreamBuffering = false;
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }

        public bool CheckOutFile(string url, string checkoutToLocal, string lastmodified)
        {
            try
            {
                var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);
                var file = _context.Web.GetFileByServerRelativeUrl(relativeUrl);
                file.CheckOut();
                _context.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckOutFileAsync(string url, string checkoutToLocal, string lastmodified)
        {
            return await Task.Factory.StartNew(() => CheckOutFile(url, checkoutToLocal, null));
        }

        public bool CheckInFile(string url, string comment, string checkinType)
        {
            try
            {
                var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);
                var file = _context.Web.GetFileByServerRelativeUrl(relativeUrl);
                file.CheckIn(comment, (CheckinType)Enum.Parse(typeof(CheckinType), checkinType));
                _context.ExecuteQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CheckInFileAsync(string url, string comment, string checkinType)
        {
            return await Task.Factory.StartNew(() => CheckInFile(url, comment, checkinType));
        }

        public string GetCurrentUser()
        {
            try
            {
                var currentUser = _context.Web.CurrentUser;
                _context.Load(currentUser);
                _context.ExecuteQuery();
                return currentUser.LoginName.Contains("|") ? currentUser.LoginName.Split('|')[1] : currentUser.LoginName;
            }
            catch (Exception)
            {
                // try/catch for just debug only, dont handle error here
                throw;
            }
        }

        public async Task<string> GetCurrentUserAsync()
        {
            return await Task.Factory.StartNew(() => GetCurrentUser());
        }

        public SharePointFile GetSharePointFile(string url)
        {
            try
            {
                var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);
                var file = _context.Web.GetFileByServerRelativeUrl(relativeUrl);
                _context.Load(file);
                _context.Load(file, t => t.ListItemAllFields, t => t.ListItemAllFields.ParentList,
                    t => t.ListItemAllFields.ParentList.RootFolder.ServerRelativeUrl);
                _context.Load(file, t => t.Author);
                _context.ExecuteQuery();
                if (file.Exists)
                {
                    return new SharePointFile()
                    {
                        RelativeUrl = file.ServerRelativeUrl,
                        Name = file.Name,
                        CreatedDateTime = file.TimeCreated,
                        LastModifiedDateTime = file.TimeLastModified,
                        Author = file.Author == null
                            ? ""
                            : (file.Author.LoginName.Contains("|")
                                ? file.Author.LoginName.Split('|')[1]
                                : file.Author.LoginName),
                        FileSize = file.Length,
                        FileExtension =
                            file.ListItemAllFields.FieldValues["File_x0020_Type"] == null
                                ? ""
                                : file.ListItemAllFields.FieldValues["File_x0020_Type"].ToString(),
                        Url = SiteSoapClientMapper.RootUrl.CombineUrl(file.ServerRelativeUrl),
                        CheckoutUser = GetCheckOutUserName(file.CheckedOutByUser),
                        UIVersionLabel = file.UIVersionLabel,
                        ParentSharePointLibrary = new SharePointLibrary()
                        {
                            EnableVersioning = file.ListItemAllFields.ParentList.EnableVersioning,
                            EnableMinorVersions = file.ListItemAllFields.ParentList.EnableMinorVersions,
                            Title = file.ListItemAllFields.ParentList.Title,
                            RelativeUrl = file.ListItemAllFields.ParentList.RootFolder.ServerRelativeUrl,
                            RequireCheckout = file.ListItemAllFields.ParentList.ForceCheckout
                        }
                    };
                }
                return null;
            }
            catch (ServerException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
                throw;
            }
            return null;
        }

        public SharePointLibrary GetParentSharePointLibraryByFileUrl(string fileUrl)
        {
            try
            {
                var relativeUrl = fileUrl.Replace(SiteSoapClientMapper.RootUrl, string.Empty);
                var file = _context.Web.GetFileByServerRelativeUrl(relativeUrl);
                var parentList = file.ListItemAllFields.ParentList;
                _context.Load(file);
                _context.Load(parentList);
                _context.Load(parentList, s => s.RootFolder.ServerRelativeUrl);
                _context.ExecuteQuery();
                if (file.Exists)
                {
                    return new SharePointLibrary()
                    {
                        Description = parentList.Description,
                        EnableVersioning = parentList.EnableVersioning,
                        RelativeUrl = parentList.RootFolder.ServerRelativeUrl,
                        Title = parentList.Title,
                        RequireCheckout = parentList.ForceCheckout
                    };
                }
                return null;
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        private string GetFileExtensionFromName(string name)
        {
            var ex = name.LastIndexOf(".", StringComparison.Ordinal);
            return name.Substring(ex + 1);
        }

        private string GetCheckOutUserName(User user)
        {
            try
            {
                return user.LoginName == null ? "" : (user.LoginName.Contains("|") ? user.LoginName.Split('|')[1] : user.LoginName);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}