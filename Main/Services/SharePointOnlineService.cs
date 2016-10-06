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
    public class SharePointOnlineService : ISharePointService
    {
        private readonly ClientContext context;
        public ClearTextCredential Credential { get; private set; }
        public SharePointServerVersion SharePointServerVersion { get; private set; }
        private readonly SharePointOnlineCredentials sharePointOnlineCredentials;
        public SiteSoapClientMapper SiteSoapClientMapper { get; private set; }
        public SharePointOnlineService
            (ClientContext context, 
            ClearTextCredential credential, 
            SiteSoapClientMapper siteSoapClientMapper, 
            SharePointServerVersion sharePointServerVersion)
        {
            SiteSoapClientMapper = siteSoapClientMapper;
            this.context = context;
            SharePointServerVersion = sharePointServerVersion;
            var securityString = new SecureString();
            foreach (char c in credential.Password) securityString.AppendChar(c);
            try
            {
                context.Credentials = new SharePointOnlineCredentials(credential.Username, securityString);
            }
            catch (Exception ex)
            {
                // SharePointOnlineCredentials will auto validate the credential and will throw exception 
                // if user input not real data, not real sharepoint online account
                // this prevent the application is crashed
            }
            sharePointOnlineCredentials = context.Credentials as SharePointOnlineCredentials;
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
                var site = context.Web;
                context.Load(site);
                context.Load(site, s => s.CurrentUser.LoginName);

                context.ExecuteQuery();
                return new SharePointSite()
                {
                    RelativeUrl = site.ServerRelativeUrl,
                    Id = site.Id,
                    Url = site.Url,
                    //RootUrl =
                    //    site.Url.LastIndexOf(site.ServerRelativeUrl) < 0
                    //        ? site.Url
                    //        : site.Url.Remove(site.Url.LastIndexOf(site.ServerRelativeUrl),
                    //            site.ServerRelativeUrl.Length),
                    //RootUrl = site.Url.Replace(site.ServerRelativeUrl, ""),
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
                var subSites = context.Web.Webs;
                context.Load(subSites, s => s.Include(t => t.Url), s => s.Include(t => t.Description),
                    s => s.Include(t => t.Title), s => s.Include(t => t.ServerRelativeUrl));
                context.ExecuteQuery();
                return subSites.ToList().Select(s => new SharePointSite()
                {
                    Url = s.Url,
                    Description = s.Description,
                    Title = s.Title,
                    //RootUrl = s.Url.LastIndexOf(s.ServerRelativeUrl) < 0
                    //            ? s.Url
                    //            : s.Url.Remove(s.Url.LastIndexOf(s.ServerRelativeUrl),
                    //                s.ServerRelativeUrl.Length),
                    RelativeUrl = s.ServerRelativeUrl
                });
            }
            catch (Exception)
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
                s => s.Include(t => t.EnableVersioning),
                s => s.Include(t => t.ForceCheckout));
            context.ExecuteQuery();
            var result =
                lists.ToList()
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

        public async Task<IEnumerable<SharePointLibrary>> GetSharePointLibrariesAsync()
        {
            return await Task.Factory.StartNew(() => GetSharePointLibraries());
        }

        /// <summary>
        /// Not need Implement for SharePoint online
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
            var list = context.Web.Lists.GetByTitle(title);
            var folders = list.GetItems(new CamlQuery()
            {
                ViewXml = "<View><Query><Where><Eq><FieldRef Name='ContentType' /><Value Type='Computed'>Folder</Value></Eq></Where></Query></View>"
            });
            //var folders = list.GetItems(CamlQuery.CreateAllFoldersQuery());

            context.Load(folders, s => s.Include(t => t.DisplayName),
                s => s.Include(t => t.Folder.Properties),
                s => s.Include(t => t.Folder.ServerRelativeUrl));
            context.ExecuteQuery();
            return folders.ToList().Select(s => new SharePointFolder()
            {
                Title = s.DisplayName,
                //Url = s.ParentList.ParentWeb.Url + s.Folder.ServerRelativeUrl,
                RelativeUrl = s.Folder.ServerRelativeUrl,
                Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.Folder.ServerRelativeUrl),
                //RootUrl = s.ParentList.ParentWeb.Url,
                Name = s.DisplayName,
                CreatedDateTime = (DateTime)s.Folder.Properties["vti_timecreated"],
                LastModifiedDateTime = (DateTime)s.Folder.Properties["vti_timelastmodified"],
                // RootUrl = SiteSoapClientMapper.RootUrl
            });
        }

        public async Task<IEnumerable<SharePointFolder>> GetSharePointFolderAsync(string title)
        {
            return await Task.Factory.StartNew(() => GetSharePointFolders(title));
        }

        ///// <summary>
        ///// Get Folders of a Library by Library Id
        ///// </summary>
        ///// <param name="guid"></param>
        ///// <returns></returns>
        //public IEnumerable<SharePointFolder> GetSharePointFolder(Guid guid)
        //{
        //    var list = context.Web.Lists.GetById(guid);
        //    var subFolders = list.GetItems(new CamlQuery()
        //    {
        //        ViewXml = "<View><Query><Where><Eq><FieldRef Name='ContentType' /><Value Type='Computed'>Folder</Value></Eq></Where></Query></View>"
        //    });

        //    context.Load(subFolders, s => s.Include(t => t.DisplayName));
        //    context.ExecuteQuery();
        //    return subFolders.ToList().Select(s => new SharePointFolder()
        //    {
        //        Title = s.DisplayName,
        //        Name = s.DisplayName,
        //        Url = SiteSoapClientMapper.RootUrl + s.Folder.ServerRelativeUrl,
        //        RelativeUrl = s.Folder.ServerRelativeUrl
        //        //RelativeUrl = s.re
        //    });

        //}

        //public IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, string listTitle)
        //{
        //    var list = context.Web.Lists.GetByTitle(listTitle);
        //    var folder = list.GetItemById(folderId);
        //    context.Load(folder, s => s.Folder.Folders, s => s.Folder.ServerRelativeUrl);
        //    context.Load(list, s => s.RootFolder.ServerRelativeUrl);

        //    context.ExecuteQuery();
        //    var subFolders = folder.Folder.Folders;
        //    //throw new NotImplementedException();
        //    return subFolders.ToList().Select(s => new SharePointFolder()
        //    {
        //        Description = "",
        //        RelativeUrl = s.ServerRelativeUrl,
        //        Title = s.Name,
        //        Name = s.Name,
        //        Url = SiteSoapClientMapper.RootUrl + s.ServerRelativeUrl
        //    });
        //}

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
            var list = context.Web.Lists.GetByTitle(listTitle);
            var files = list.RootFolder.Files;
            var result = new List<ISharePointFile>();
            try
            {
                context.Load(files, s => s.Include(t => t.Author), s => s.Include(t => t.ServerRelativeUrl),
                s => s.Include(t => t.Name), s => s.Include(t => t.TimeCreated), s => s.Include(t => t.TimeLastModified),
                s => s.Include(t => t.Length), s => s.Include(t => t.ListItemAllFields));
                context.Load(files, s => s.Include(t => t.CheckedOutByUser));
                context.Load(files, s => s.Include(t => t.UIVersionLabel));
                context.ExecuteQuery();
                files.ToList().ForEach(s =>
                {
                    var newFile = new SharePointFile
                    {
                        RelativeUrl = s.ServerRelativeUrl,
                        Name = s.Name,
                        CreatedDateTime = (DateTime)s.ListItemAllFields.FieldValues["Created"],
                        LastModifiedDateTime = (DateTime)s.ListItemAllFields.FieldValues["Modified"],
                        Author = s.Author == null ? "" : s.Author.LoginName.Split('|')[2],
                        FileSize = s.Length,
                        FileExtension =
                            s.ListItemAllFields.FieldValues["File_x0020_Type"] == null
                                ? ""
                                : s.ListItemAllFields.FieldValues["File_x0020_Type"].ToString(),
                        Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ServerRelativeUrl),
                        CheckoutUser = GetCheckOutUserName(s.CheckedOutByUser),
                        UIVersionLabel = s.UIVersionLabel
                    };
                    result.Add(newFile);
                });
            }
            catch (Exception ex)
            {
                context.Load(files, s => s.Include(t => t.ServerRelativeUrl),
                s => s.Include(t => t.Name), s => s.Include(t => t.TimeCreated), s => s.Include(t => t.TimeLastModified),
                s => s.Include(t => t.Length), s => s.Include(t => t.ListItemAllFields["File_x0020_Type"]));
                // context.Load(files, s => s.Include(t => t.Author));
                context.Load(files, s => s.Include(t => t.CheckedOutByUser));
                context.Load(files, s => s.Include(t => t.UIVersionLabel));
                context.ExecuteQuery();

                files.ToList().ForEach(s =>
                {
                    var newFile = new SharePointFile
                    {
                        RelativeUrl = s.ServerRelativeUrl,
                        Name = s.Name,
                        CreatedDateTime = s.TimeCreated,
                        LastModifiedDateTime = s.TimeLastModified,
                        //Author = s.Author == null ? "" : s.Author.Title,
                        FileSize = s.Length,
                        FileExtension = !s.ListItemAllFields.FieldValues.ContainsKey("File_x0020_Type")
                            ? GetFileExtensionFromName(s.Name)
                            : s.ListItemAllFields["File_x0020_Type"].ToString(),
                        Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ServerRelativeUrl),
                        CheckoutUser = GetCheckOutUserName(s.CheckedOutByUser),
                        UIVersionLabel = s.UIVersionLabel
                    };
                    result.Add(newFile);
                });
            }

            
            

            return result;
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
            var folder = context.Web.GetFolderByServerRelativeUrl(relativeUrl);
            var files = folder.Files;
            context.Load(files, s => s.Include(t => t.ListItemAllFields));
            context.Load(files, s => s.Include(t => t.Author));
            context.ExecuteQuery();

            return files.ToList().Select(s => new SharePointFile()
            {
                RelativeUrl = s.ListItemAllFields["FileRef"].ToString(),
                Name = s.ListItemAllFields["FileLeafRef"].ToString(),
                CreatedDateTime = (DateTime) s.ListItemAllFields["Created"],
                LastModifiedDateTime = (DateTime) s.ListItemAllFields["Modified"],
                FileSize = long.Parse(s.ListItemAllFields["File_x0020_Size"].ToString()),
                Author =
                    s.Author == null
                        ? ""
                        : (s.Author.LoginName.Contains("|") ? s.Author.LoginName.Split('|')[2] : s.Author.LoginName),
                //Author =
                //    s.Author == null
                //        ? ""
                //        : s.Author.LoginName.Split('|')[2],
                FileExtension =
                    s.ListItemAllFields["File_x0020_Type"] == null
                        ? ""
                        : s.ListItemAllFields["File_x0020_Type"].ToString(),
                Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.ListItemAllFields["FileRef"].ToString()),
                UIVersionLabel = s.ListItemAllFields["_UIVersionString"].ToString(),
                CheckoutUser = GetCheckOutUserName(s.CheckedOutByUser)
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
            // This can download current files only
            //var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);
            //var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);
            //context.Load(file, s => s.Name, s => s.ServerRelativeUrl, s => s.Versions, s => s.UIVersionLabel, s => s.UIVersion);
            //context.ExecuteQuery();
            //var finfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, relativeUrl);
            //return finfo.Stream;

            // This will download the file by url, both for current version or a history version
            var absoluteUrl = url;
            var request = (HttpWebRequest)WebRequest.Create(absoluteUrl);
            request.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            request.Credentials = sharePointOnlineCredentials;
            request.Timeout = 10000;
            request.AllowWriteStreamBuffering = false;
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }

        public async Task<Stream> DownloadFileByUrlAsync(string url)
        {
            var absoluteUrl = url;
            var request = (HttpWebRequest)WebRequest.Create(absoluteUrl);
            request.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            request.Credentials = sharePointOnlineCredentials;
            request.Timeout = 10000;
            request.AllowWriteStreamBuffering = false;
            var response = (HttpWebResponse)(await request.GetResponseAsync());
            return response.GetResponseStream();
        }

        public List<CopyActionResult> UploadFile(string parentRelativeUrl, string name, Stream streamFile)
        {
            if (context.HasPendingRequest)
            context.ExecuteQuery();
            try
            {
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, parentRelativeUrl + "/" + name, streamFile, true);
                return new List<CopyActionResult>(){new CopyActionResult()
                {
                    ErrorCode = ErrorCode.Success,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(parentRelativeUrl, name)
                }};
            }
            catch (Exception ex)
            {
                return new List<CopyActionResult>() { new CopyActionResult() { ErrorCode = ErrorCode.Unknown, Message = ex.Message} };
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
            // throw new Exception("Test exception");
            // var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);

            var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);

            context.Load(file, s => s.Name, s => s.ServerRelativeUrl, s => s.Author.LoginName, s => s.ListItemAllFields, s => s.TimeCreated,
                s =>
                    s.Versions.Include(t => t.Created, t => t.Url, t => t.CheckInComment, t => t.CreatedBy, t => t.ID,
                        t => t.ID, t => t.Size, t => t.VersionLabel), s => s.UIVersionLabel, s => s.UIVersion,
                s => s.TimeLastModified, s => s.Length, s => s.ModifiedBy);
            context.ExecuteQuery();

            var timeZonoOffset = (DateTime)file.ListItemAllFields.FieldValues["Created"] - file.TimeCreated;

            var result = file.Versions.ToList().Select(s => new SharePointFileVersion()
            {
                Url = SiteSoapClientMapper.Url.CombineUrl(s.Url),
                RelativeUrl = s.Url,
                CheckInComment = s.CheckInComment,
                Created = s.Created + timeZonoOffset,
                CreatedBy = s.CreatedBy.LoginName.Split(new []{"|"}, StringSplitOptions.None)[2],
                ID = s.ID,
                Size = s.Size,
                VersionLabel = s.VersionLabel,
                // Url = Libs.Utils.CombineUrl(SiteSoapClientMapper.RootUrl, s.Url)
            }).ToList();

            var currentVersion = new SharePointFileVersion
            {
                Url = url,
                RelativeUrl = relativeUrl,
                Created = (DateTime)file.ListItemAllFields.FieldValues["Modified"],
                // CreatedBy = file.ModifiedBy.LoginName.Split(new[] { "|" }, StringSplitOptions.None)[2],
                CreatedBy = file.Author == null
                        ? ""
                        : (file.Author.LoginName.Contains("|") ? file.Author.LoginName.Split('|')[2] : file.Author.LoginName),
                // RootUrl = SiteSoapClientMapper.RootUrl,
                Size = file.Length,
                CheckInComment = "",
                OriginalFileName = file.Name,
                VersionLabel = file.UIVersionLabel
            };
            result.Add(currentVersion);

            return result;
        }

        public async Task<IEnumerable<SharePointFileVersion>> GetVersionsAsync(string url, string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetVersions(url, relativeUrl));
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="url"></param>
        ///// <param name="listId">Not used</param>
        ///// <param name="listItemId">Not used</param>
        ///// <returns></returns>
        //public IEnumerable<SharePointFileVersion> GetVersions(string url, string listId, string listItemId)
        //{
        //    var relativeUrl = url.Replace(SiteSoapClientMapper.RootUrl, string.Empty);

        //    var file = context.Web.GetFileByServerRelativeUrl(relativeUrl);

        //    context.Load(file, s => s.Name, s => s.ServerRelativeUrl,
        //        s =>
        //            s.Versions.Include(t => t.Created, t => t.Url, t => t.CheckInComment, t => t.CreatedBy, t => t.ID,
        //                t => t.ID, t => t.Size, t => t.VersionLabel), s => s.UIVersionLabel, s => s.UIVersion);
        //    context.ExecuteQuery();

        //    var result = file.Versions.ToList().Select(s => new SharePointFileVersion()
        //    {
        //        Url = SiteSoapClientMapper.RootUrl + s.Url,
        //        RelativeUrl = s.Url,
        //        CheckInComment = s.CheckInComment,
        //        Created = s.Created,
        //        CreatedBy = s.CreatedBy.LoginName.Split(new[] { "\\" }, StringSplitOptions.None)[1],
        //        ID = s.ID,
        //        Size = s.Size,
        //        VersionLabel = s.VersionLabel
        //    });

        //    return result;
        //}

        public Stream DownloadHistoryVersion(string url)
        {
            var absoluteUrl = url;
            var request = (HttpWebRequest)WebRequest.Create(absoluteUrl);
            request.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            request.Credentials = sharePointOnlineCredentials;
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
            var currentUser = context.Web.CurrentUser;
            context.Load(currentUser);
            context.ExecuteQuery();
            return currentUser.LoginName.Split('|')[2];
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
                        Author = file.Author == null
                            ? ""
                            : (file.Author.LoginName.Contains("|")
                                ? file.Author.LoginName.Split('|')[1]
                                : file.Author.LoginName),
                        FileSize = file.Length,
                        FileExtension =
                            file.ListItemAllFields.FieldValues.ToList()[30].Value == null
                                ? ""
                                : file.ListItemAllFields.FieldValues.ToList()[30].Value.ToString(),
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
                return user.LoginName.Split('|')[2];
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
