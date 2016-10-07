using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;
using Core.Libs;
using Entities;
using Main.Libs;
using Main.Services.Domains;
using Microsoft.SharePoint.Client;
using Services;

namespace Main.Services
{
    public class SharePoint2010ServiceClientObjModel : ISharePointService
    {
        private readonly ClientContext context;
        public ClearTextCredential Credential { get; private set; }
        public SharePointServerVersion SharePointServerVersion { get; private set; }
        public SiteSoapClientMapper SiteSoapClientMapper { get; private set; }
        public SharePoint2010ServiceClientObjModel
            (ClientContext context,
            ClearTextCredential credential,
            SiteSoapClientMapper siteSoapClientMapper,
            SharePointServerVersion sharePointServerVersion,
            bool office365 = false)
        {
            SiteSoapClientMapper = siteSoapClientMapper;
            this.context = context;
            SharePointServerVersion = sharePointServerVersion;
            if (office365)
            {
                var securityString = new SecureString();
                foreach (var c in credential.Password) securityString.AppendChar(c);
                try
                {
                    context.Credentials = new SharePointOnlineCredentials(credential.Username, securityString);
                }
                catch (Exception)
                {
                    // SharePointOnlineCredentials will auto validate the credential and will throw exception 
                    // if user input not real data, not real sharepoint online account
                    // this prevent the application is crashed
                }

            }
            else
            {
                context.Credentials = new NetworkCredential(credential.Username, credential.Password, credential.DomainName);
            }
            Credential = credential;
        }

        public TryConnectResult TestConnection()
        {
            try
            {
                var web = context.Web;
                context.Load(web);
                context.ExecuteQuery();
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
                var site = context.Site;
                context.Load(site, s => s.Url);
                var web = context.Web;
                context.Load(web);
                context.Load(web, s => s.CurrentUser.LoginName);

                context.ExecuteQuery();
                return new SharePointSite()
                {
                    RelativeUrl = web.ServerRelativeUrl,
                    Id = web.Id,
                    Url = site.Url + web.ServerRelativeUrl,
                    // RootUrl = site.Url,
                    //RootUrl = site.Url.Replace(site.ServerRelativeUrl, ""),
                    Title = web.Title,
                    User =
                        web.CurrentUser.LoginName.Contains("|")
                            ? web.CurrentUser.LoginName.Split('|')[1]
                            : web.CurrentUser.LoginName,
                    Description = web.Description
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SharePointSite> GetSharePointSiteAsync()
        {
            return await Task.Factory.StartNew(GetSharePointSite);
        }

        /// <summary>
        /// Get sub sites of current site
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SharePointSite> GetSubSharePointSites()
        {
            try
            {
                var subSites = context.Web.Webs;
                context.Load(subSites, s => s.Include(t => t.Description),
                    s => s.Include(t => t.Title), s => s.Include(t => t.ServerRelativeUrl));
                context.ExecuteQuery();
                var sites = subSites.ToList().Select(s => new SharePointSite()
                {
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ServerRelativeUrl),
                    Description = s.Description,
                    Title = s.Title,
                    //RootUrl = s.Url.LastIndexOf(s.ServerRelativeUrl) < 0
                    //            ? s.Url
                    //            : s.Url.Remove(s.Url.LastIndexOf(s.ServerRelativeUrl),
                    //                s.ServerRelativeUrl.Length),
                    RelativeUrl = s.ServerRelativeUrl
                });
                return sites;
            }
            catch (Exception ex)
            {
                return new List<SharePointSite>();
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
            var lists = context.Web.Lists;
            context.Load(lists, s => s.Include(t => t.RootFolder.ServerRelativeUrl), s => s.Include(t => t.Hidden),
                s => s.Include(t => t.Description), s => s.Include(t => t.Title),
                //s => s.Include(t => t.ParentWeb),
                //s => s.Include(t => t.IsApplicationList),
                s => s.Include(t => t.BaseTemplate),
                // s => s.Include(t => t.BaseType),
                s => s.Include(t => t.EnableVersioning),
                s => s.Include(t => t.ForceCheckout));
            context.ExecuteQuery();
            var result =
                lists.ToList()
                    .Where(
                        s => !s.Hidden && ((ListTemplateType)s.BaseTemplate == ListTemplateType.DocumentLibrary))
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

        public async Task<IEnumerable<SharePointLibrary>> GetSharePointLibrariesAsync()
        {
            return await Task.Factory.StartNew(() => GetSharePointLibraries());
        }

        /// <summary>
        /// Not need Implement for SharePoint 2010
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
                var list = context.Web.Lists.GetByTitle(title);
                var folders = list.GetItems(new CamlQuery()
                {
                    ViewXml = "<View><Query><Where><Eq><FieldRef Name=\"FSObjType\" /><Value Type=\"Integer\">1</Value></Eq></Where></Query></View>"
                });

                context.Load(folders);
                context.ExecuteQuery();

                var result = 
                folders.ToList().Select(s => new SharePointFolder()
                {
                    Title = s.FieldValues["Title"] == null ? "" : s.FieldValues["Title"].ToString(),
                    RelativeUrl = s.FieldValues["FileRef"].ToString(),
                    Name = s.FieldValues["FileLeafRef"].ToString(),
                    CreatedDateTime = (DateTime)s.FieldValues["Created"],
                    LastModifiedDateTime = (DateTime)s.FieldValues["Modified"],
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.FieldValues["FileRef"].ToString())
                }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public async Task<IEnumerable<SharePointFolder>> GetSharePointFolderAsync(string title)
        {
            return await Task.Factory.StartNew(() => GetSharePointFolders(title));
        }

        public IEnumerable<SharePointFolder> GetSharePointSubFolder(string relativeUrl)
        {
            var parentFolder = context.Web.GetFolderByServerRelativeUrl(relativeUrl);
            var folders = parentFolder.Folders;
            context.Load(folders, s => s.Include(t => t.ServerRelativeUrl), s => s.Include(t => t.Name));
            context.ExecuteQuery();
            return folders.ToList().Select(s => new SharePointFolder()
            {
                Title = s.Name,
                Name = s.Name,
                RelativeUrl = s.ServerRelativeUrl,
                Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ServerRelativeUrl)
            });
        }

        public async Task<IEnumerable<SharePointFolder>> GetSharePointSubFolderAsync(string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetSharePointSubFolder(relativeUrl));
        }

        ///// <summary>
        ///// Get sub folders of a folder by parent folder id. 
        ///// </summary>
        ///// <param name="folderId"></param>
        ///// <param name="listId"></param>
        ///// <returns></returns>
        //public IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, Guid listId)
        //{
        //    var list = context.Web.Lists.GetById(listId);
        //    var folder = list.GetItemById(folderId);
        //    context.Load(folder, s => s.Folder);
        //    context.ExecuteQuery();
        //    var subFolders = folder.Folder.Folders;
        //    throw new NotImplementedException();
        //}


        /// <summary>
        /// Get file of a Library
        /// </summary>
        /// <param name="listTitle">Library title</param>
        /// <returns></returns>
        public IEnumerable<ISharePointFile> GetSharePointFiles(string listTitle)
        {
            try
            {
                var lists = context.Web.Lists;
                var list = lists.GetByTitle(listTitle);
                var result = new List<ISharePointFile>();
                var files = list.RootFolder.Files;

                context.Load(files, s => s.Include(t => t.ListItemAllFields));
                // context.Load(files, s => s.Include(t => t.UIVersionLabel));

                context.ExecuteQuery();
                files.ToList().ForEach(s =>
                {
                    var values = s.ListItemAllFields.FieldValues;
                    var newFile = new SharePointFile
                    {
                        RelativeUrl = values["FileRef"].ToString(),
                        Url = SiteSoapClientMapper.RootUrl.CombineUrl(values["FileRef"].ToString()),
                        Name = values["FileLeafRef"] == null ? "" : values["FileLeafRef"].ToString(),
                        CreatedDateTime = (DateTime) values["Created"],
                        LastModifiedDateTime = (DateTime) values["Modified"],
                        Author =
                            !(values["Author"] is FieldUserValue)
                                ? ""
                                : ((FieldUserValue) values["Author"]).LookupValue,
                        FileSize = long.Parse(values["File_x0020_Size"].ToString()),
                        FileExtension = values["File_x0020_Type"] == null
                            ? ""
                            : values["File_x0020_Type"].ToString(),
                        CheckoutUser =
                            !(values["CheckoutUser"] is FieldUserValue)
                                ? ""
                                : ((FieldUserValue) values["CheckoutUser"]).LookupValue,
                        UIVersionLabel = values["_UIVersionString"].ToString()
                    };
                    result.Add(newFile);
                });
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IEnumerable<ISharePointFile>> GetSharePointFilesAsync(string listTitle)
        {
            var result = await Task.Factory.StartNew(() => GetSharePointFiles(listTitle));
            return result;
        }

        /// <summary>
        /// Get file of a Library
        /// </summary>
        /// <param name="relativeUrl">Library relativeUrl</param>
        /// <returns></returns>
        public IEnumerable<ISharePointFile> GetSharePointFilesInFolder(string relativeUrl)
        {
            var folder = context.Web.GetFolderByServerRelativeUrl(relativeUrl);
            var files = folder.Files;
            context.Load(files, s => s.Include(t => t.ListItemAllFields));
            context.ExecuteQuery();

            return files.ToList().Select(s => new SharePointFile()
            {
                RelativeUrl = s.ListItemAllFields["FileRef"].ToString(),
                Name = s.ListItemAllFields["FileLeafRef"].ToString(),
                CreatedDateTime = (DateTime) s.ListItemAllFields["Created"],
                LastModifiedDateTime = (DateTime) s.ListItemAllFields["Modified"],
                FileSize = long.Parse(s.ListItemAllFields["File_x0020_Size"].ToString()),
                Author =
                    s.ListItemAllFields["Author"] == null
                        ? ""
                        : (s.ListItemAllFields["Author"] as FieldUserValue).LookupValue,
                FileExtension =
                    s.ListItemAllFields["File_x0020_Type"] == null
                        ? ""
                        : s.ListItemAllFields["File_x0020_Type"].ToString(),
                Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ListItemAllFields["FileRef"].ToString()),
                UIVersionLabel = s.ListItemAllFields["_UIVersionString"].ToString(),
                CheckoutUser =
                    s.ListItemAllFields["CheckoutUser"] == null
                        ? ""
                        : (s.ListItemAllFields["CheckoutUser"] as FieldUserValue).LookupValue
            });
        }

        public async Task<IEnumerable<ISharePointFile>> GetSharePointFilesInFolderAsync(string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetSharePointFilesInFolder(relativeUrl));
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="relativeUrl"></param>
        ///// <returns></returns>
        //public Stream DownloadFile(string relativeUrl)
        //{
        //    var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
        //    context.Load(file, s => s.Name, s => s.ServerRelativeUrl, s => s.Versions, s => s.UIVersionLabel, s => s.UIVersion);
        //    context.ExecuteQuery();
        //    var finfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, relativeUrl);
        //    return finfo.Stream;
        //}

        public Stream DownloadFileByUrl(string url)
        {
            //var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);

            //var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
            //context.Load(file, s => s.Name, s => s.ServerRelativeUrl, s => s.Versions, s => s.UIVersionLabel, s => s.UIVersion);
            //context.ExecuteQuery();
            //var finfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, relativeUrl);
            //return finfo.Stream;
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
            if (context.HasPendingRequest)
            context.ExecuteQuery();
            try
            {
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, parentRelativeUrl.CombineUrl(name), streamFile, true);
                return new List<CopyActionResult>(){new CopyActionResult()
                {
                    ErrorCode = ErrorCode.Success,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(parentRelativeUrl.CombineUrl(name))
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
            if (context.HasPendingRequest)
            context.ExecuteQuery();
            try
            {
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, relativeUrl, streamFile, true);
                return new List<CopyActionResult>(){new CopyActionResult()
                {
                    ErrorCode = ErrorCode.Success,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(absoluteUrl)
                }};
            }
            catch (Exception ex)
            {
                return new List<CopyActionResult>() { new CopyActionResult() { ErrorCode = ErrorCode.Unknown, Message = ex.Message } };
            }
        }

        public bool CheckFileExist(string parentRelativeUrl, string name)
        {
            var parentFolder = context.Web.GetFolderByServerRelativeUrl(parentRelativeUrl);
            var files = parentFolder.Files;
            var exFiles = context.LoadQuery(files.Where(s => s.Name == name));
            context.ExecuteQuery();
            return exFiles.Any();
        }

        //public void RefreshCredential()
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<SharePointFileVersion> GetVersions(string url, string relativeUrl)
        {
            try
            {
                var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
                var versions = file.Versions;
                var listItemAllFields = file.ListItemAllFields;
                context.Load(versions);
                context.Load(versions, t => t.Include(s => s.CreatedBy));
                context.Load(listItemAllFields);
                context.Load(file, s => s.TimeCreated, s => s.TimeLastModified, s => s.Name, s => s.UIVersionLabel);
                context.ExecuteQuery();

                var timeZonoOffset = (DateTime) file.ListItemAllFields.FieldValues["Created"] - file.TimeCreated;

                var result = versions.ToList().Select(s => new SharePointFileVersion()
                {
                    Url = SiteSoapClientMapper.Url.CombineUrl(s.Url),
                    RelativeUrl = s.Url,
                    CheckInComment = s.CheckInComment,
                    Created = s.Created + timeZonoOffset,
                    CreatedBy = s.CreatedBy == null ? string.Empty : s.CreatedBy.LoginName,
                    ID = s.ID,
                    VersionLabel = s.VersionLabel,
                }).ToList();

                var values = listItemAllFields.FieldValues;

                var currentVersion = new SharePointFileVersion
                {
                    Url = url,
                    RelativeUrl = relativeUrl,
                    Created = (DateTime)file.ListItemAllFields.FieldValues["Modified"],
                    CreatedBy = values["Created_x0020_By"].ToString(),
                    Size = long.Parse(values["File_x0020_Size"].ToString()),
                    CheckInComment = "",
                    OriginalFileName = file.Name,
                    VersionLabel = file.UIVersionLabel
                };
                result.Add(currentVersion);

                return result;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public async Task<IEnumerable<SharePointFileVersion>> GetVersionsAsync(string url, string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetVersions(url, relativeUrl));
        }

        public Stream DownloadHistoryVersion(string url)
        {
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
                var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
                file.CheckOut();
                context.ExecuteQuery();
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
                var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
                file.CheckIn(comment, (CheckinType)Enum.Parse(typeof(CheckinType), checkinType));
                context.ExecuteQuery();
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
                var currentUser = context.Web.CurrentUser;
                context.Load(currentUser);
                context.ExecuteQuery();
                return currentUser.LoginName;
            }
            catch (Exception)
            {
                return string.Empty;
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
                var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
                context.Load(file);
                context.Load(file, t => t.ListItemAllFields, t => t.ListItemAllFields.ParentList);
                context.Load(file, t => t.Author);
                context.ExecuteQuery();
                if (file.Exists)
                {
                    return new SharePointFile()
                    {
                        RelativeUrl = file.ServerRelativeUrl,
                        Name = file.Name,
                        CreatedDateTime = file.TimeCreated,
                        LastModifiedDateTime = file.TimeLastModified,
                        Author = file.Author == null ? "" : file.Author.LoginName,
                        FileSize = long.Parse(file.ListItemAllFields.FieldValues["File_x0020_Size"].ToString()),
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
                            EnableMinorVersions = file.ListItemAllFields.ParentList.EnableMinorVersions
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
                //MessageBox.Show(ex.Message);
                throw;
            }
            return null;
        }

        public SharePointLibrary GetParentSharePointLibraryByFileUrl(string fileUrl)
        {
            try
            {
                var relativeUrl = fileUrl.Replace(SiteSoapClientMapper.RootUrl, string.Empty);
                var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
                var parentList = file.ListItemAllFields.ParentList;
                context.Load(file);
                context.Load(parentList);
                context.Load(parentList, s => s.RootFolder.ServerRelativeUrl);
                context.ExecuteQuery();
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
                return user.LoginName.Split('|')[1];
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}