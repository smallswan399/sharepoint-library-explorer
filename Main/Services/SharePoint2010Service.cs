using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Core;
using Main.Copy2010ServiceReference;
using Main.Libs;
using Main.List2010ServiceReference;
using Main.Services.Domains;
using Main.Services.Serializations;
using Main.Version2010ServiceReference;
using Main.Web2010ServiceReference;
using Services;
using Services.Serializations;

namespace Main.Services
{
    [Obsolete("", true)]
    public class SharePoint2010Service : ISharePointService
    {
        private readonly ListsSoapClient listsSoapClient;
        private readonly WebsSoapClient websSoapClient;
        private readonly CopySoapClient copySoapClient;
        private readonly VersionsSoapClient versionsSoapClient;

        public SiteSoapClientMapper SiteSoapClientMapper { get; private set; }
        public ClearTextCredential Credential { get; private set; }
        public SharePointServerVersion SharePointServerVersion { get; private set; }

        public SharePoint2010Service
            (WebsSoapClient websSoapClient, 
            ListsSoapClient listsSoapClient, 
            CopySoapClient copySoapClient, 
            VersionsSoapClient versionsSoapClient, 
            ClearTextCredential credential, 
            SiteSoapClientMapper siteSoapClientMapper, 
            SharePointServerVersion sharePointServerVersion)
        {
            SharePointServerVersion = sharePointServerVersion;
            this.websSoapClient = websSoapClient;
            this.listsSoapClient = listsSoapClient;
            Credential = credential;
            SiteSoapClientMapper = siteSoapClientMapper;
            this.copySoapClient = copySoapClient;
            this.versionsSoapClient = versionsSoapClient;
        }

        public SharePoint2010Service(ClearTextCredential credential)
        {
            
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

        public IEnumerable<SharePointSite> GetSubSharePointSites()
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
                RootUrl = SiteSoapClientMapper.RootUrl,
                RelativeUrl = s.Url.Replace(SiteSoapClientMapper.RootUrl, string.Empty)
            });
        }



        public IEnumerable<SharePointLibrary> GetSharePointLibraries()
        {
            var sub =
                listsSoapClient.GetListCollection()
                    .Elements()
                    .Select(s => s.FromXElement<List>("http://schemas.microsoft.com/sharepoint/soap/")).ToList();


            //var test = sub.Where(s => s.Hidden == "False" && s.IsApplicationList == "False");
            //var libs = listsSoapClient.GetList("Shared Documents");
            var result = sub.Where(s => s.Hidden == "False" && s.IsApplicationList == "False" && s.BaseType == "1")
                .Select(s => new SharePointLibrary()
            {
                Description = s.Description,
                Title = s.Title,
                Id = s.ID,
                RootUrl = SiteSoapClientMapper.RootUrl
            });


            return result;
        }

        public SharePointLibrary GetSharePointLibrary(string title)
        {
            var list =
                listsSoapClient.GetList(title).FromXElement<ListDetails>("http://schemas.microsoft.com/sharepoint/soap/");
                    //.Select(s => s.FromXElement<ListDetails>("http://schemas.microsoft.com/sharepoint/soap/"))
                    //.ToList();
            if (list != null)
            {
                return new SharePointLibrary()
                {
                    RootUrl = SiteSoapClientMapper.RootUrl,
                    RelativeUrl = list.RelativeUrl,
                    Description = list.Description,
                    Id = list.ID,
                    Title = list.Title
                };
            }
            return null;
        }

        public IEnumerable<SharePointFolder> GetSharePointFolder(string title)
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

            string query = oSb.ToString();

            var oSb1 = new StringBuilder();

            oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb1.Append("</ViewFields>");

            string fields = oSb1.ToString();

            var folders = listsSoapClient.GetListItems(title, null, XElement.Parse(query), XElement.Parse(fields), null, null, null);
            var result = folders.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).Select(s => new SharePointFolder
            {
                Title = s.Attribute(XName.Get("ows_Title")).Value,
                CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                Name = s.Attribute(XName.Get("ows_Title")).Value,
                RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                RootUrl = SiteSoapClientMapper.Url
            });

            return result.ToList();
        }

        public IEnumerable<SharePointFolder> GetSharePointFolder(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, Guid listId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, string listTitle)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SharePointFolder> GetSharePointSubFolder(string relativeUrl)
        {
            // Get List name from relative Url
            var listName = relativeUrl.Split('/')[1];

            var absoluteUrl = SiteSoapClientMapper.Url + relativeUrl;


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
                Title = s.Attribute(XName.Get("ows_Title")).Value,
                CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                Name = s.Attribute(XName.Get("ows_Title")).Value,
                RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                RootUrl = SiteSoapClientMapper.Url
            });

            return result.ToList();
        }

        public IEnumerable<ISharePointFile> GetSharePointFiles(string url)
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

            var oSb1 = new StringBuilder();

            oSb1.Append("<ViewFields xmlns=\"http://schemas.microsoft.com/sharepoint/soap/\">");
            oSb1.Append("</ViewFields>");

            string fields = oSb1.ToString();

            var files = listsSoapClient.GetListItems(url, null, XElement.Parse(query), XElement.Parse(fields), null, null, null);
            //var file = files.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).First();
            //var dateTime = file.Attribute(XName.Get("ows_Created")).Value;
            //var name =
            //    file.Attribute(XName.Get("ows_FileLeafRef"))
            //        .Value.Substring(file.Attribute(XName.Get("ows_FileLeafRef")).Value.LastIndexOf("#") + 1);
            //var RelativeUrl = file.Attribute(XName.Get("ows_ServerUrl")).Value;
            //var LastModifiedDateTime = (DateTime.Parse(file.Attribute(XName.Get("ows_Modified")).Value));
            //var Author =
            //    file.Attribute(XName.Get("ows_Author"))
            //        .Value.Substring(file.Attribute(XName.Get("ows_Author")).Value.LastIndexOf("#") + 1);
            //var FileExtension = file.Attribute(XName.Get("ows_File_x0020_Type")) == null ? "" : file.Attribute(XName.Get("ows_File_x0020_Type")).Value;
            //var FileSize = file.Attribute(XName.Get("ows_File_x0020_Size")) == null ? 0 :
            //    (long.Parse(
            //        file.Attribute(XName.Get("ows_File_x0020_Size"))
            //            .Value.Substring(file.Attribute(XName.Get("ows_File_x0020_Size")).Value.LastIndexOf("#") + 1)));





            var result = files.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).Select(s => new SharePointFile
            {
                CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                Name = s.Attribute(XName.Get("ows_FileLeafRef")).Value.Substring(s.Attribute(XName.Get("ows_FileLeafRef")).Value.LastIndexOf("#") + 1),
                RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                Author = s.Attribute(XName.Get("ows_Author")).Value.Substring(s.Attribute(XName.Get("ows_Author")).Value.LastIndexOf("#") + 1),
                FileExtension = s.Attribute(XName.Get("ows_File_x0020_Type")) == null ? string.Empty : s.Attribute(XName.Get("ows_File_x0020_Type")).Value,
                FileSize = s.Attribute(XName.Get("ows_File_x0020_Size")) == null ? 0 : (long.Parse(s.Attribute(XName.Get("ows_File_x0020_Size")).Value.Substring(s.Attribute(XName.Get("ows_File_x0020_Size")).Value.LastIndexOf("#") + 1)))
            });

            return result.ToList();
        }

        public IEnumerable<ISharePointFile> GetSharePointFilesInFolder(string relativeUrl)
        {
            // Get List name from relative Url
            var listName = relativeUrl.Split('/')[1];

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

            var files = listsSoapClient.GetListItems(listName, null, XElement.Parse(query), XElement.Parse(fields), null, XElement.Parse(queryOption), null);
            var result = files.DescendantsAndSelf(XName.Get("row", "#RowsetSchema")).Select(s => new SharePointFile()
            {
                CreatedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Created")).Value)),
                Name = s.Attribute(XName.Get("ows_FileLeafRef")).Value.Substring(s.Attribute(XName.Get("ows_FileLeafRef")).Value.LastIndexOf("#") + 1),
                RelativeUrl = s.Attribute(XName.Get("ows_ServerUrl")).Value,
                LastModifiedDateTime = (DateTime.Parse(s.Attribute(XName.Get("ows_Modified")).Value)),
                Author = s.Attribute(XName.Get("ows_Author")).Value.Substring(s.Attribute(XName.Get("ows_Author")).Value.LastIndexOf("#") + 1),
                FileExtension = s.Attribute(XName.Get("ows_File_x0020_Type")).Value,
                FileSize = (long.Parse(s.Attribute(XName.Get("ows_File_x0020_Size")).Value.Substring(s.Attribute(XName.Get("ows_File_x0020_Size")).Value.LastIndexOf("#") + 1)))
            });

            return result.ToList();
        }

        public Stream DownloadFile(string relativeUrl)
        {
            throw new NotImplementedException();
            //var absoluteUrl = SiteSoapClientMapper.Url + relativeUrl;
            //var request = (HttpWebRequest)WebRequest.Create(absoluteUrl);

            //request.Credentials = new NetworkCredential(Credential.Username, Credential.Password, Credential.DomainName);
            //request.Timeout = 10000;
            //request.AllowWriteStreamBuffering = false;
            //var response = (HttpWebResponse)request.GetResponse();
            //return response.GetResponseStream();
        }

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

        public List<CopyActionResult> UploadFile(string parentRelativeUrl, string name, Stream streamFile)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(Credential.Username, Credential.Password, Credential.DomainName);
                    var newBytes = new byte[streamFile.Length];
                    streamFile.Read(newBytes, 0, newBytes.Length);
                    var destinationUrl = Utils.RemoveAllSlashAtFinish(SiteSoapClientMapper.RootUrl) + parentRelativeUrl + "/" + name;
                    client.UploadData(destinationUrl, "PUT", newBytes);
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
            //var newBytes = new byte[streamFile.Length];
            //streamFile.Read(newBytes, 0, newBytes.Length);
            //var info = new[]{new FieldInformation
            //            {
            //                DisplayName = name,
            //                Id = Guid.NewGuid(),
            //                InternalName = name,
            //                Type = FieldType.File,
            //                Value = name
            //            }};

            //var destinationUrl = Utils.RemoveAllSlashAtFinish(SiteSoapClientMapper.RootUrl) + parentRelativeUrl + "/" + name;
            //CopyResult[] results;
            //copySoapClient.CopyIntoItems(Uri.EscapeUriString(destinationUrl), new[] { Uri.EscapeUriString(destinationUrl) }, info, newBytes, out results);
            //return results.Select(s => new CopyActionResult
            //{
            //    Message = s.ErrorMessage,
            //    ErrorCode = CopyActionResult.GetErrorCode2010(s.ErrorCode)
            //}).ToList();
        }

        public List<CopyActionResult> UploadFile(string absoluteUrl, Stream streamFile)
        {
            throw new NotImplementedException();
        }

        public bool CheckFileExist(string parentRelativeUrl, string name)
        {
            return false;
        }

        public void RefreshCredential()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SharePointFileVersion> GetVersions(string url)
        {
            var result = versionsSoapClient.GetVersions(url)
                .DescendantsAndSelf(XName.Get("result", "http://schemas.microsoft.com/sharepoint/soap/"))
                .Select(s => new SharePointFileVersion
                {
                    CheckInComment = s.Attribute(XName.Get("comments")).Value,
                    Created = DateTime.Parse(s.Attribute(XName.Get("created")).Value),
                    CreatedBy = s.Attribute(XName.Get("createdBy")).Value,
                    Size = int.Parse(s.Attribute(XName.Get("size")).Value),
                    Url = s.Attribute(XName.Get("url")).Value,
                    VersionLabel = s.Attribute(XName.Get("version")).Value
                }).Where(s => s.Url != url);
            return result;
        }

        public IEnumerable<SharePointFileVersion> GetVersions(string url, string listId, string listItemId)
        {
            throw new NotImplementedException();
        }

        public Stream DownloadHistoryVersion(string url)
        {
            throw new NotImplementedException();
        }

        public bool CheckOutFile(string url, string checkoutToLocal, string lastmodified)
        {
            throw new NotImplementedException();
        }

        public bool CheckInFile(string url, string comment, string checkinType)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public SharePointFile GetSharePointFile(string url)
        {
            throw new NotImplementedException();
        }
    }
}