using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Core;
using Core.Libs;
using Entities;
using Main.CopyServiceReference;
using Main.Libs;
using Main.ListServiceReference;
using Main.Services.Domains;
using Main.Services.Serializations;
using Main.SiteDataServiceReference;
using Main.SitesServiceReference;
using Main.VersionServiceReference;
using Main.WebServiceReference;
using Services;
using Services.Serializations;

namespace Main.Services
{
    public class SharePoint2007Service : ISharePointService
    {
        private readonly ListsSoapClient listsSoapClient;
        private readonly WebsSoapClient websSoapClient;
        //private readonly CopySoapClient copySoapClient;
        private readonly VersionsSoapClient versionsSoapClient;
        private readonly SitesSoapClient sitesSoapClient;
        private readonly SiteDataSoapClient siteDataSoapClient;
        private readonly UserProfileService.UserProfileService userProfileService;
        public SiteSoapClientMapper SiteSoapClientMapper { get; private set; }
        public ClearTextCredential Credential { get; private set; }
        public SharePointServerVersion SharePointServerVersion { get; private set; }
        //private string webUrl;

        public SharePoint2007Service
            (WebsSoapClient websSoapClient, 
            ListsSoapClient listsSoapClient, 
            CopySoapClient copySoapClient, 
            VersionsSoapClient versionsSoapClient,
            UserProfileService.UserProfileService userProfileService,
            SitesSoapClient sitesSoapClient,
            SiteDataSoapClient siteDataSoapClient,
            ClearTextCredential credential, 
            SiteSoapClientMapper siteSoapClientMapper, 
            SharePointServerVersion sharePointServerVersion)
        {
            SharePointServerVersion = sharePointServerVersion;
            this.siteDataSoapClient = siteDataSoapClient;
            this.sitesSoapClient = sitesSoapClient;
            this.userProfileService = userProfileService;
            this.versionsSoapClient = versionsSoapClient;
            this.websSoapClient = websSoapClient;
            this.listsSoapClient = listsSoapClient;
            Credential = credential;
            SiteSoapClientMapper = siteSoapClientMapper;
            //this.copySoapClient = copySoapClient;
            //this.webUrl = siteSoapClientMapper.Url;
        }

        public async Task<IEnumerable<SharePointSite>> GetSubSharePointSitesAsync()
        {
            return await Task.Factory.StartNew(() => GetSubSharePointSites());
        }

        /// <summary>
        /// Return a collection of Sites or Libraries at a URL
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SharePointLibrary> GetSharePointLibraries()
        {
            var sub =
                listsSoapClient.GetListCollection()
                    .Elements()
                    .Select(s => s.FromXElement<List>("http://schemas.microsoft.com/sharepoint/soap/")).ToList();

            var result = sub.Where(s => s.Hidden == "False" && s.BaseType == "1").Select(s =>
            {
                // Can we improve this? This request server in loop :(
                var relativeUrl = GetSharePointLibrary(s.Title).RelativeUrl;
                return new SharePointLibrary()
                {
                    Description = s.Description,
                    Title = s.Title,
                    Id = s.ID,
                    // RootUrl = SiteSoapClientMapper.RootUrl,
                    EnableVersioning = bool.Parse(s.EnableVersioning),
                    RequireCheckout = bool.Parse(s.RequireCheckout),
                    RelativeUrl = relativeUrl,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(relativeUrl),
                    EnableMinorVersions = bool.Parse(s.EnableMinorVersion)
                };
            });

            return result;
        }

        public async Task<IEnumerable<SharePointLibrary>> GetSharePointLibrariesAsync()
        {
            return await Task.Factory.StartNew(() => GetSharePointLibraries());
        }

        public SharePointLibrary GetSharePointLibrary(string title)
        {
            var list =
                listsSoapClient.GetList(title)
                    .FromXElement<ListDetails>("http://schemas.microsoft.com/sharepoint/soap/");
            if (list != null)
            {
                return new SharePointLibrary()
                {
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(list.RelativeUrl),
                    RelativeUrl = list.RelativeUrl,
                    Description = list.Description,
                    Id = list.ID,
                    Title = list.Title
                };
            }
            return null;
        }

        public IEnumerable<SharePointFolder> GetSharePointFolders(string title)
        {
            var oSb = new StringBuilder();

            oSb.Append("<Query xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb.Append("<Where>");
            oSb.Append("<Eq>");
            oSb.Append("<FieldRef Name=\"FSObjType\" />");
            oSb.Append("<Value Type=\"Text\">1</Value>");
            oSb.Append("</Eq>");
            oSb.Append("</Where>");
            oSb.Append("<OrderBy>");
            oSb.Append("<FieldRef Name=\"FileLeafRef\" />");
            oSb.Append("</OrderBy>");
            oSb.Append("</Query>");

            var query = oSb.ToString();

            var oSb1 = new StringBuilder();

            oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb1.Append("</ViewFields>");

            var fields = oSb1.ToString();

            var folders = listsSoapClient.GetListItems(title, null, XElement.Parse(query), XElement.Parse(fields), null, null, null);
            var result = folders.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).Select(s => new SharePointFolder
            {
                Title = s.Attribute(XName.Get("ows_FileLeafRef")).Value.Split('#')[1],
                CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                Name = s.Attribute(XName.Get("ows_FileLeafRef")).Value.Split('#')[1],
                RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.Attribute(XName.Get("ows_ServerUrl")).Value),
                LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                // RootUrl = SiteSoapClientMapper.RootUrl,
                Author = s.Attribute(XName.Get("ows_Author")).Value.Split('#')[1]
            });

            return result.ToList();
        }

        public async Task<IEnumerable<SharePointFolder>> GetSharePointFolderAsync(string title)
        {
            return await Task.Factory.StartNew(() => GetSharePointFolders(title));
        }

        //public IEnumerable<SharePointFolder> GetSharePointFolder(Guid guid)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, Guid listId)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, string listTitle)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeUrl">Parent relative Url</param>
        /// <returns></returns>
        public IEnumerable<SharePointFolder> GetSharePointSubFolder(string relativeUrl)
        {
            try
            {

                var list = GetSharePointLibraries().FirstOrDefault(s => relativeUrl.StartsWith(s.RelativeUrl));
                if (list == null)
                {
                    // Or throw an exception?
                    return new List<SharePointFolder>();
                }
                var listName = list.Title;
                // Get List name from relative Url
                // var listName = (SiteSoapClientMapper.RootUrl + relativeUrl).Replace(SiteSoapClientMapper.Url, "").Split('/')[1];

                // var absoluteUrl = SiteSoapClientMapper.RootUrl + relativeUrl;
                var oSb = new StringBuilder();

                oSb.Append("<Query xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
                oSb.Append("<Where>");
                oSb.Append("<Eq>");
                oSb.Append("<FieldRef Name=\"FSObjType\" />");
                oSb.Append("<Value Type=\"Text\">1</Value>");
                oSb.Append("</Eq>");
                oSb.Append("</Where>");
                oSb.Append("<OrderBy>");
                oSb.Append("<FieldRef Name=\"FileLeafRef\" />");
                oSb.Append("</OrderBy>");
                oSb.Append("</Query>");

                var queryOptionBuilder = new StringBuilder();
                queryOptionBuilder.Append("<QueryOptions>");
                queryOptionBuilder.Append("<Folder>");
                queryOptionBuilder.Append(relativeUrl);
                queryOptionBuilder.Append("</Folder>");
                queryOptionBuilder.Append("</QueryOptions>");

                string query = oSb.ToString();
                string queryOption = queryOptionBuilder.ToString();

                var oSb1 = new StringBuilder();

                oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
                oSb1.Append("</ViewFields>");

                string fields = oSb1.ToString();

                var folders = listsSoapClient.GetListItems(listName, null, XElement.Parse(query), XElement.Parse(fields), null, XElement.Parse(queryOption), null);
                var result = folders.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).Select(s => new SharePointFolder
                {
                    Title = s.Attribute(XName.Get("ows_FileLeafRef")).Value.Split('#')[1],
                    CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                    Name = s.Attribute(XName.Get("ows_FileLeafRef")).Value.Split('#')[1],
                    RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                    LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                    Url = SiteSoapClientMapper.Url + s.Attribute(XName.Get("ows_ServerUrl")).Value
                });

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

        }

        public async Task<IEnumerable<SharePointFolder>> GetSharePointSubFolderAsync(string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetSharePointSubFolder(relativeUrl));
        }

        public IEnumerable<SharePointFolder> GetSharePointSubFolder(int id)
        {
            throw new NotImplementedException();
        }

        public TryConnectResult TestConnection()
        {
            try
            {
                websSoapClient.GetWeb(SiteSoapClientMapper.Url);
                return new TryConnectResult()
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
            // var result = new SharePointSite();
            var web =
                websSoapClient.GetWeb(SiteSoapClientMapper.Url)
                    .FromXElement<Web>("http://schemas.microsoft.com/sharepoint/soap/");
            
            string site;
            string strWeb;
            siteDataSoapClient.GetSiteAndWeb(SiteSoapClientMapper.Url, out site, out strWeb);
            string siteId;
            siteDataSoapClient.GetSiteUrl(SiteSoapClientMapper.Url, out site, out siteId);
            var result = new SharePointSite()
            {
                Description = web.Description,
                LocalId = SiteSoapClientMapper.Id,
                Id = new Guid(siteId),
                Url = strWeb,
                // RootUrl = site,
                RelativeUrl = strWeb.Replace(site, ""),
                Title = web.Title,
                User = GetCurrentUser()
            };
            return result;
        }

        public async Task<SharePointSite> GetSharePointSiteAsync()
        {
            return await Task.Factory.StartNew(() => GetSharePointSite());
        }

        public IEnumerable<SharePointSite> GetSubSharePointSites()
        {
            try
            {
                var sub =
                websSoapClient.GetWebCollection()
                    .Elements()
                    .Select(s => s.FromXElement<Web>("http://schemas.microsoft.com/sharepoint/soap/")).ToList();
                return sub.Select(s => new SharePointSite()
                {
                    Url = s.Url,
                    Description = s.Description,
                    Title = s.Title,

                    // Need to check to fill RootUrl and in caller code
                    //RootUrl = SiteSoapClientMapper.Url,
                    //RelativeUrl = s.Url.Replace(SiteSoapClientMapper.Url, string.Empty)
                });
            }
            catch (Exception)
            {
                return new List<SharePointSite>();
                throw;
            }
        }

        public IEnumerable<ISharePointFile> GetSharePointFiles(string listTitle)
        {
            // GetAll(listTitle);
            var oSb = new StringBuilder();
            oSb.Append("<Query xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb.Append("<Where>");
            oSb.Append("<Eq>");
            oSb.Append("<FieldRef Name=\"FSObjType\" />");
            oSb.Append("<Value Type=\"Text\">0</Value>");
            oSb.Append("</Eq>");
            oSb.Append("</Where>");
            oSb.Append("<OrderBy>");
            oSb.Append("<FieldRef Name=\"FileLeafRef\" />");
            oSb.Append("</OrderBy>");
            oSb.Append("</Query>");
            var query = oSb.ToString();

            var oSb1 = new StringBuilder();
            oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb1.Append("</ViewFields>");
            string fields = oSb1.ToString();

            var files = listsSoapClient.GetListItems(listTitle, null, XElement.Parse(query), XElement.Parse(fields), null, null, null);

            var result = new List<SharePointFile>();
            files.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).ToList().ForEach(s =>
            {
                var file = new SharePointFile
                {
                    CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                    Id = int.Parse(s.Attribute(XName.Get("ows_ID")).Value),
                    Guid = Guid.Parse(s.Attribute(XName.Get("ows_GUID")).Value),
                    Name =
                        s.Attribute(XName.Get("ows_FileLeafRef"))
                            .Value.Substring(s.Attribute(XName.Get("ows_FileLeafRef")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                    RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                    Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.Attribute(XName.Get("ows_ServerUrl")).Value),
                    LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                    Author =
                        s.Attribute(XName.Get("ows_Author"))
                            .Value.Substring(s.Attribute(XName.Get("ows_Author")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                    FileExtension =
                        s.Attribute(XName.Get("ows_File_x0020_Type")) == null
                            ? ""
                            : s.Attribute(XName.Get("ows_File_x0020_Type")).Value,
                    FileSize =
                        (long.Parse(
                            s.Attribute(XName.Get("ows_File_x0020_Size"))
                                .Value.Substring(s.Attribute(XName.Get("ows_File_x0020_Size")).Value.LastIndexOf("#", StringComparison.Ordinal) +
                                                 1))),
                    CheckoutUser =
                        s.Attribute(XName.Get("ows_CheckoutUser")) == null
                            ? null
                            : s.Attribute(XName.Get("ows_CheckoutUser"))
                                .Value.Substring(s.Attribute(XName.Get("ows_CheckoutUser")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                    UIVersionLabel =
                        s.Attribute(XName.Get("ows__UIVersionString")) == null
                            ? null
                            : s.Attribute(XName.Get("ows__UIVersionString")).Value.ToString()
                };
                result.Add(file);
            });

            return result;
        }

        public async Task<IEnumerable<ISharePointFile>> GetSharePointFilesAsync(string listTitle)
        {
            return await Task.Factory.StartNew(() => GetSharePointFiles(listTitle));
        }

        private void GetAll(string listTitle)
        {
            var oSb = new StringBuilder();

            oSb.Append("<Query xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb.Append("<Where>");
            oSb.Append("<Eq>");
            oSb.Append("<FieldRef Name=\"FSObjType\" />");
            oSb.Append("<Value Type=\"Text\">0</Value>");
            oSb.Append("</Eq>");
            oSb.Append("</Where>");
            oSb.Append("<OrderBy>");
            oSb.Append("<FieldRef Name=\"FileLeafRef\" />");
            oSb.Append("</OrderBy>");
            oSb.Append("</Query>");

            string query = oSb.ToString();

            //var oSb1 = new StringBuilder();

            //oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            //oSb1.Append("</ViewFields>");

            //string fields = oSb1.ToString();

            var queryOptionBuilder = new StringBuilder();
            queryOptionBuilder.Append("<QueryOptions>");
            queryOptionBuilder.Append("<ViewAttributes Scope=\"RecursiveAll\" />");
            //queryOptionBuilder.Append("<Folder>");
            //queryOptionBuilder.Append(relativeUrl);
            //queryOptionBuilder.Append("</Folder>");
            queryOptionBuilder.Append("</QueryOptions>");

            var oSb1 = new StringBuilder();

            oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb1.Append("</ViewFields>");

            var files = listsSoapClient.GetListItems(listTitle, null, XElement.Parse(query), XElement.Parse(oSb1.ToString()), null, XElement.Parse(queryOptionBuilder.ToString()), null);
            return;
        }

        public IEnumerable<ISharePointFile> GetSharePointFilesInFolder(string relativeUrl)
        {
            // Need improve this?
            var list = GetSharePointLibraries().FirstOrDefault(s => relativeUrl.StartsWith(s.RelativeUrl));
            if (list == null)
            {
                // Or throw an exception?
                return new List<SharePointFile>();
            }
            var listName = list.Title;
            // var listName = (SiteSoapClientMapper.RootUrl + relativeUrl).Replace(SiteSoapClientMapper.Url, "").Split('/')[1];

            var absoluteUrl = SiteSoapClientMapper.Url + relativeUrl;


            var oSb = new StringBuilder();

            oSb.Append("<Query xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb.Append("<Where>");
            oSb.Append("<Eq>");
            oSb.Append("<FieldRef Name=\"FSObjType\" />");
            oSb.Append("<Value Type=\"Text\">0</Value>");
            oSb.Append("</Eq>");
            oSb.Append("</Where>");
            oSb.Append("<OrderBy>");
            oSb.Append("<FieldRef Name=\"FileLeafRef\" />");
            oSb.Append("</OrderBy>");
            oSb.Append("</Query>");

            var queryOptionBuilder = new StringBuilder();
            queryOptionBuilder.Append("<QueryOptions>");
            queryOptionBuilder.Append("<Folder>");
            queryOptionBuilder.Append(relativeUrl);
            queryOptionBuilder.Append("</Folder>");
            queryOptionBuilder.Append("</QueryOptions>");

            string query = oSb.ToString();
            string queryOption = queryOptionBuilder.ToString();

            var oSb1 = new StringBuilder();

            oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb1.Append("</ViewFields>");

            string fields = oSb1.ToString();

            var files = listsSoapClient.GetListItems(listName, null, XElement.Parse(query), XElement.Parse(fields), null,
                XElement.Parse(queryOption), null);
            var result = files.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).Select(s => new SharePointFile()
            {
                CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                Name =
                    s.Attribute(XName.Get("ows_FileLeafRef"))
                        .Value.Substring(s.Attribute(XName.Get("ows_FileLeafRef")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                Url = SiteSoapClientMapper.RootUrl.CombineUrl(s.Attribute(XName.Get("ows_ServerUrl")).Value),
                LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                Author =
                    s.Attribute(XName.Get("ows_Author"))
                        .Value.Substring(s.Attribute(XName.Get("ows_Author")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                FileExtension =
                    s.Attribute(XName.Get("ows_File_x0020_Type")) == null
                        ? ""
                        : s.Attribute(XName.Get("ows_File_x0020_Type")).Value,
                FileSize =
                    (long.Parse(
                        s.Attribute(XName.Get("ows_File_x0020_Size"))
                            .Value.Substring(s.Attribute(XName.Get("ows_File_x0020_Size")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1))),
                CheckoutUser =
                    s.Attribute(XName.Get("ows_CheckoutUser")) == null
                        ? null
                        : s.Attribute(XName.Get("ows_CheckoutUser"))
                            .Value.Substring(s.Attribute(XName.Get("ows_CheckoutUser")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                UIVersionLabel =
                    s.Attribute(XName.Get("ows__UIVersionString")) == null
                        ? null
                        : s.Attribute(XName.Get("ows__UIVersionString")).Value.ToString()
            });

            return result.ToList();
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
        //    throw new NotImplementedException();
        //}

        public Stream DownloadFileByUrl(string url)
        {
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
            var absoluteUrl = url;
            var request = (HttpWebRequest)WebRequest.Create(absoluteUrl);

            request.Credentials = new NetworkCredential(Credential.Username, Credential.Password, Credential.DomainName);
            request.Timeout = 10000;
            request.AllowWriteStreamBuffering = false;
            var response = (HttpWebResponse)(await request.GetResponseAsync());
            return response.GetResponseStream();
        }

        public List<CopyActionResult> UploadFile(string parentRelativeUrl, string name, Stream streamFile)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(Credential.Username, Credential.Password, Credential.DomainName);
                    var newBytes = new byte[streamFile.Length];
                    streamFile.Read(newBytes, 0, newBytes.Length);
                    var destinationUrl = SiteSoapClientMapper.RootUrl.CombineUrl(parentRelativeUrl, name);
                    client.Headers.Add("Overwrite", "T");
                    var response = client.UploadData(destinationUrl, "PUT", newBytes);

                    //listsSoapClient.UpdateListItems()

                    return new List<CopyActionResult>()
                    {
                        new CopyActionResult()
                        {
                            ErrorCode = ErrorCode.Success,
                            Url = destinationUrl
                        }
                    };
                }
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
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(Credential.Username, Credential.Password, Credential.DomainName);
                    var newBytes = new byte[streamFile.Length];
                    streamFile.Read(newBytes, 0, newBytes.Length);
                    var destinationUrl = absoluteUrl;
                    client.Headers.Add("Overwrite", "T");
                    var response = client.UploadData(destinationUrl, "PUT", newBytes);

                    //listsSoapClient.UpdateListItems()

                    return new List<CopyActionResult>() { new CopyActionResult()
                    {
                        ErrorCode = ErrorCode.Success,
                        Url = destinationUrl
                    } };
                }
            }
            catch (Exception ex)
            {
                return new List<CopyActionResult>() { new CopyActionResult() { ErrorCode = ErrorCode.Unknown, Message = ex.Message } };
            }
        }

        public bool CheckFileExist(string parentRelativeUrl, string name)
        {
            throw new NotImplementedException();
        }

        //public void RefreshCredential()
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<SharePointFileVersion> GetVersions(string url, string relativeUrl)
        {
            var result = versionsSoapClient.GetVersions(url)
                .DescendantsAndSelf(XName.Get("result", "http://schemas.microsoft.com/sharepoint/soap/"))
                .Select(s => new SharePointFileVersion
                {
                    CheckInComment = s.Attribute(XName.Get("comments")).Value,
                    Created = DateTime.ParseExact(s.Attribute(XName.Get("created")).Value, "M/d/yyyy h:mm tt", null),
                    CreatedBy = s.Attribute(XName.Get("createdBy")).Value,
                    Size = int.Parse(s.Attribute(XName.Get("size")).Value),
                    // this is absolute url
                    Url = s.Attribute(XName.Get("url")).Value,
                    RelativeUrl = s.Attribute(XName.Get("url")).Value.Replace(SiteSoapClientMapper.RootUrl, ""),
                    VersionLabel = s.Attribute(XName.Get("version")).Value
                }).ToList();
            // Remove @ character
            result.ForEach(s =>
            {
                if (s.VersionLabel.Contains("@"))
                {
                    s.VersionLabel = s.VersionLabel.Replace("@", "");
                }
            });
            return result;
        }

        public async Task<IEnumerable<SharePointFileVersion>> GetVersionsAsync(string url, string relativeUrl)
        {
            return await Task.Factory.StartNew(() => GetVersions(url, relativeUrl));
        }

        //public IEnumerable<SharePointFileVersion> GetVersions(string url, string listId, string listItemId)
        //{
        //    var resdult = listsSoapClient.GetVersionCollection(listId, listItemId, "_UIVersionString");
        //    //throw new NotImplementedException();
        //    //var resuldt = versionsSoapClient.GetVersions(url);
        //    //var result = listsSoapClient.GetVersionCollection(listId, listItemId, "_UIVersionString");
        //    //var result3 = listsSoapClient.GetVersionCollection(listId, listItemId, "_IsCurrentVersion");
        //    return new List<SharePointFileVersion>();
        //}

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
            return listsSoapClient.CheckOutFile(url, checkoutToLocal, null);
            //return listsSoapClient.CheckOutFile(url, checkoutToLocal, lastmodified);
        }

        public async Task<bool> CheckOutFileAsync(string url, string checkoutToLocal, string lastmodified)
        {
            return await Task.Factory.StartNew(() => CheckOutFile(url, checkoutToLocal, null));
        }

        public bool CheckInFile(string url, string comment, string checkinType)
        {
            return listsSoapClient.CheckInFile(url, comment, checkinType);
        }

        public async Task<bool> CheckInFileAsync(string url, string comment, string checkinType)
        {
            return await Task.Factory.StartNew(() => CheckInFile(url, comment, checkinType));
        }

        public string GetCurrentUser()
        {
            var currentProfile = userProfileService.GetUserProfileByName(string.Empty);
            return currentProfile.Count() > 1 ? currentProfile[1].Values[0].Value.ToString() : string.Empty;
        }

        public async Task<string> GetCurrentUserAsync()
        {
            return await Task.Factory.StartNew(() => GetCurrentUser());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">file Url that not contain version label</param>
        /// <returns></returns>
        public SharePointFile GetSharePointFile(string url)
        {
            // Url = Root url + Relative Url of list + file name + version label
            var urlSplited = url.Split('/');
            var fileNameLen = urlSplited[urlSplited.Length - 1].Length;
            var rootUrl = SiteSoapClientMapper.RootUrl;
            var fileRelativeUrl = url.Replace(rootUrl, "");
            var listRelativeUrl = fileRelativeUrl.Substring(0, fileRelativeUrl.Length - fileNameLen - 1);
            
            //var listName = url.Replace(SiteSoapClientMapper.Url, "").Split('/')[0];

            // Specific list name
            var list = GetSharePointLibraries().FirstOrDefault(s => s.RelativeUrl == listRelativeUrl);

            var oSb = new StringBuilder();

            oSb.Append("<Query xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb.Append("<Where>");
            oSb.Append("<Eq>");
            oSb.Append("<FieldRef Name=\"FSObjType\" />");
            oSb.Append("<Value Type=\"Text\">0</Value>");
            oSb.Append("</Eq>");
            oSb.Append("<Eq>");
            oSb.Append("<FieldRef Name=\"FSObjType\" />");
            oSb.Append("<Value Type=\"Text\">0</Value>");
            oSb.Append("</Eq>");
            oSb.Append("</Where>");
            oSb.Append("<OrderBy>");
            oSb.Append("<FieldRef Name=\"FileLeafRef\" />");
            oSb.Append("</OrderBy>");
            oSb.Append("</Query>");

            string query = oSb.ToString();

            var queryOptionBuilder = new StringBuilder();
            queryOptionBuilder.Append("<QueryOptions>");
            queryOptionBuilder.Append("<ViewAttributes Scope=\"RecursiveAll\" />");
            queryOptionBuilder.Append("</QueryOptions>");

            var oSb1 = new StringBuilder();

            oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb1.Append("</ViewFields>");

            var files = listsSoapClient.GetListItems(list.Title, null, XElement.Parse(query), XElement.Parse(oSb1.ToString()), null, XElement.Parse(queryOptionBuilder.ToString()), null);

            var result = new List<SharePointFile>();
            files.DescendantsAndSelf(XName.Get("row", "#RowsetSchema"))
                .Where(s => s.Attribute(XName.Get("ows_ServerUrl")).Value == fileRelativeUrl)
                .ToList().ForEach(s =>
            {
                var file = new SharePointFile
                {
                    CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                    Id = int.Parse(s.Attribute(XName.Get("ows_ID")).Value),
                    Guid = Guid.Parse(s.Attribute(XName.Get("ows_GUID")).Value),
                    Name =
                        s.Attribute(XName.Get("ows_FileLeafRef"))
                            .Value.Substring(s.Attribute(XName.Get("ows_FileLeafRef")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                    RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                    LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                    Author =
                        s.Attribute(XName.Get("ows_Author"))
                            .Value.Substring(s.Attribute(XName.Get("ows_Author")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                    FileExtension =
                        s.Attribute(XName.Get("ows_File_x0020_Type")) == null
                            ? ""
                            : s.Attribute(XName.Get("ows_File_x0020_Type")).Value,
                    FileSize =
                        (long.Parse(
                            s.Attribute(XName.Get("ows_File_x0020_Size"))
                                .Value.Substring(s.Attribute(XName.Get("ows_File_x0020_Size")).Value.LastIndexOf("#", StringComparison.Ordinal) +
                                                 1))),
                    CheckoutUser =
                        s.Attribute(XName.Get("ows_CheckoutUser")) == null
                            ? null
                            : s.Attribute(XName.Get("ows_CheckoutUser"))
                                .Value.Substring(s.Attribute(XName.Get("ows_CheckoutUser")).Value.LastIndexOf("#", StringComparison.Ordinal) + 1),
                    UIVersionLabel =
                        s.Attribute(XName.Get("ows__UIVersionString")) == null
                            ? null
                            : s.Attribute(XName.Get("ows__UIVersionString")).Value.ToString()
                };
                result.Add(file);
            });

            return result.FirstOrDefault();
        }

        public SharePointLibrary GetParentSharePointLibraryByFileUrl(string fileUrl)
        {
            // Url = Root url + Relative Url of list + file name + version label
            var urlSplited = fileUrl.Split('/');
            // var labelVersionLen = urlSplited[urlSplited.Length - 1].Length;
            var fileNameLen = urlSplited[urlSplited.Length - 1].Length;
            var rootUrl = SiteSoapClientMapper.RootUrl;
            var fileRelativeUrl = fileUrl.Replace(rootUrl, "");
            var listRelativeUrl = fileRelativeUrl.Substring(0, fileRelativeUrl.Length - fileNameLen - 1);

            //var listName = url.Replace(SiteSoapClientMapper.Url, "").Split('/')[0];

            // Specific list name
            var list = GetSharePointLibraries().FirstOrDefault(s => s.RelativeUrl == listRelativeUrl);
            return list;
        }
    }
}