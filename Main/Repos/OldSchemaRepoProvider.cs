using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Core;
using Core.Libs;
using Entities;
using Extensions;
using Microsoft.Win32;

namespace Main.Repos
{
    public class OldSchemaRepoProvider : IRepoProvider
    {
        private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(List<Site>));
        /// <summary>
        /// this is registry key name that store the application data. This contain a sub fix
        /// </summary>
        private readonly string _rootKey;

        /// <summary>
        /// Site
        /// </summary>
        //private const string SitesKey = "sites";
        private readonly string _sitexmlPath;
        /// <summary>
        /// the key that contain url of selected file
        /// </summary>
        private const string SelectedFilesKey = "url";
        private const string SelectedFileAuthor = "author";
        private const string TreeViewPathKey = "treeviewpath";
        // private const string SelectedFilesKeyEx = "urlex";

        /// <summary>
        /// The key that contain local path of downloaded file
        /// </summary>
        private const string DownloadedFilesKey = "urldm";

        // ReSharper disable once UnusedMember.Local
        private const string UploadedFilesKey = "urldu";
        /// <summary>
        /// The key that store the result of actions
        /// </summary>
        private const string ResultKey = "iresult";
        /// <summary>
        /// The key that contain local directory is used to stored downloaded file
        /// </summary>
        private const string DownloadDirKey = "urldn";

        /// <summary>
        /// Local path of 1st file will be upload
        /// </summary>
        private const string Upload1 = "upload1";
        private const string Upload2 = "upload2";
        private const string Upload3 = "upload3";

        // ReSharper disable once UnusedMember.Local
        private const string Message = "msg";

        private readonly Data _data;

        /// <summary>
        /// A Plug-in place holder
        /// </summary>
        [Import(typeof(IAdditionalSPSite))]
        public IAdditionalSPSite AdditionalSpSite { get; set; }

        public OldSchemaRepoProvider(string rootKey)
        {
            Log.LogMessage("Construct OldSchemaRepoProvider using " + rootKey, 1);
            _rootKey = rootKey;
#if DEBUG
            _sitexmlPath = Path.Combine(Constants.Data, "sites_debug.xml");
#else
            _sitexmlPath = Path.Combine(Constants.Data, "sites.xml");
#endif
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Extensions")))
            {
                var catalog = new AggregateCatalog();
                catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(Environment.CurrentDirectory, "Extensions")));
                var container = new CompositionContainer(catalog);
                try
                {
                    container.ComposeParts(this);
                }
                catch (CompositionException compositionException)
                {
                    Console.WriteLine(compositionException.ToString());
                    Console.ReadLine();
                }
            }
            

            // Load site list into the app
            // Log.LogMessage("Load site list into the app");
            _data = InitSites();
            SiteRepository = new SiteRepository(_data);
            // Load registry data into the app
            //Log.LogMessage("Load registry data into the app");
            _data = InitDataFromRegistry(_data);
            SaveChanges(ResultMode.None);
        }

        public ISiteRepository SiteRepository { get; }
        public SelectedFiles SelectedFiles
        {
            get { return _data.SelectedFiles; }
            set { _data.SelectedFiles = value; }
        }

        public DownloadedFiles DownloadedFiles
        {
            get { return _data.DownloadedFiles; }
            set { _data.DownloadedFiles = value; }
        }

        public UploadingFiles UploadingFiles
        {
            get { return _data.UploadingFiles; }
            set { _data.UploadingFiles = value; }
        }
        public UploadedFiles UploadedFiles
        {
            get { return _data.UploadedFiles; }
            set { _data.UploadedFiles = value; }
        }

        public SelectedFileVersions SelectedFileVersions
        {
            get { return _data.SelectedFileVersions; }
            set { _data.SelectedFileVersions = value; }
        }

        public string DownloadDirectory
        {
            get { return _data.DownloadDirectory; }
            set { _data.DownloadDirectory = value; }
        }

        public int SaveChanges(ResultMode mode)
        {
            try
            {
                switch (mode)
                {
                    case ResultMode.SelectFiles:
                        if (_data.SelectedFiles.Result.Status == ResultStatus.Success)
                        {
                            // Save 1st file
                            Registry.SetValue(_rootKey, SelectedFilesKey, _data.SelectedFiles.Files.First().GetSPUrl(),
                                RegistryValueKind.String);
                            Registry.SetValue(_rootKey, TreeViewPathKey, _data.SelectedFiles.Files.First().TreeViewPath);
                            Registry.SetValue(_rootKey, SelectedFileAuthor, _data.SelectedFiles.Files.First().Author);
                            // Save the result
                            Registry.SetValue(_rootKey, ResultKey, 0, RegistryValueKind.DWord);
                        }
                        else
                        {
                            // Save the result
                            Registry.SetValue(_rootKey, ResultKey, -11, RegistryValueKind.DWord);
                        }
                        // Registry.SetValue(rootKey, SelectedFilesKeyEx, data.SelectedFiles.XmlSerializeToString());
                        break;
                    case ResultMode.DownloadFiles:
                        if (_data.DownloadedFiles.Result.Status == ResultStatus.Success)
                        {
                            Registry.SetValue(_rootKey, DownloadedFilesKey,
                                ListStringToString(_data.DownloadedFiles.Files.Select(s => s.LocalPath).ToList()));

                            Registry.SetValue(_rootKey, ResultKey, 0, RegistryValueKind.DWord);
                        }
                        else
                        {
                            Registry.SetValue(_rootKey, DownloadedFilesKey, string.Empty);
                            Registry.SetValue(_rootKey, ResultKey, -11, RegistryValueKind.DWord);
                        }
                        break;
                    case ResultMode.UploadFiles:
                        if (_data.UploadedFiles.Result.Status == ResultStatus.Success)
                        {
                            // Not save the url of uploading files
                            //Registry.SetValue(rootKey, UploadedFilesKey,
                            //    ListStringToString(data.UploadedFiles.Files.Select(s => s.Url).ToList()));
                            // Just save uploading result
                            Registry.SetValue(_rootKey, ResultKey, 0, RegistryValueKind.DWord);
                        }
                        else
                        {
                            Registry.SetValue(_rootKey, ResultKey, -11, RegistryValueKind.DWord);
                        }
                        break;
                    case ResultMode.None:
                        break;
                        //default:
                        //    throw new ArgumentOutOfRangeException("mode");
                }
                // throw new NotImplementedException();
                Registry.SetValue(_rootKey, DownloadDirKey, DownloadDirectory);
                //Registry.SetValue(rootKey, SitesKey, data.Sites.XmlSerializeToString());
                // Save site list to sites.xml file
                using (var writer = new StreamWriter(_sitexmlPath))
                {
                    _xmlSerializer.Serialize(writer, _data.Sites);
                }
                return 0;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                return 0;
            }
        }

        private Data InitSites()
        {
            List<Site> sites;
            if (File.Exists(_sitexmlPath))
            {
                try
                {
                    using (var reader = new StreamReader(_sitexmlPath))
                    {
                        sites = _xmlSerializer.Deserialize(reader) as List<Site>;
                        Log.LogMessage("Load SharePoint sites data successfully from " + _sitexmlPath, 2);
                        sites?.ForEach(s =>
                        {
                            if (s.Id == new Guid())
                            {
                                s.Id = Guid.NewGuid();
                            }

                            if (AdditionalSpSite == null) return;

                            s.SubSites = AdditionalSpSite.GetAddtionalSites(s);
                            s.SubSites.ToList().ForEach(t => t.Id = Guid.NewGuid());
                        });
                    }
                    
                }
                catch (Exception ex)
                {
                    DialogHelper.ShowErrorDialog(ex, Log.LogException(ex));
                    sites = new List<Site>();
                }
            }
            else
            {
                Log.LogMessage(_sitexmlPath + " not exist");
                sites = new List<Site>();
            }
            return new Data()
            {
                Sites = sites
            };
        }

        private Data InitDataFromRegistry(Data data)
        {
            // get SelectedFiles from registry
            var selectedFilesString = (string)Registry.GetValue(_rootKey, SelectedFilesKey, string.Empty);
            var treeViewPath = (string) Registry.GetValue(_rootKey, TreeViewPathKey, string.Empty);
            // var selectedFileVersionsString = (string)Registry.GetValue(rootKey, SelectedFileVersionsKeyName, string.Empty);
            // var downloadedFilesString = (string)Registry.GetValue(rootKey, DownloadedFilesKey, string.Empty);

            var uploadingFilesString1 = (string)Registry.GetValue(_rootKey, Upload1, string.Empty);
            var uploadingFilesString2 = (string)Registry.GetValue(_rootKey, Upload2, string.Empty);
            // ReSharper disable once UnusedVariable
            var uploadingFilesString3 = (string)Registry.GetValue(_rootKey, Upload3, string.Empty);

            // var uploadedFilesString = (string)Registry.GetValue(rootKey, UploadedFilesKeyName, string.Empty);
            var downloadDirectoryString = (string)Registry.GetValue(_rootKey, DownloadDirKey, string.Empty);
            // var sitesString = (string)Registry.GetValue(rootKey, SitesKey, string.Empty);



            data.DownloadDirectory =
                string.IsNullOrWhiteSpace(downloadDirectoryString) ? string.Empty : downloadDirectoryString;

            data.SelectedFiles = ConstructSelectedFileFromSpUrl(selectedFilesString, treeViewPath);
                
            data.UploadingFiles = string.IsNullOrWhiteSpace(uploadingFilesString1)
                ? new UploadingFiles()
                : new UploadingFiles()
                {
                    Files = new List<UploadingFile>()
                    {
                        new UploadingFile()
                        {
                            LocalPath = uploadingFilesString1,
                            VersionHolderSelectedFileFile = ConstructSelectedFileFromSpUrl(uploadingFilesString2, null).Files.FirstOrDefault()
                        }
                    }
                };

            return data;
        }

        private string ListStringToString(List<string> strList)
        {
            var result = string.Empty;
            strList.ForEach(s => result += (s + ";"));
            if (result.EndsWith(";"))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        // ReSharper disable once UnusedMember.Local
        private List<string> ToUploadingFiles(string f1, string f2, string f3)
        {
            var result = new List<string>();
            if (!string.IsNullOrWhiteSpace(f1))
            {
                result.Add(f1);
            }
            if (!string.IsNullOrWhiteSpace(f2))
            {
                result.Add(f2);
            }
            if (!string.IsNullOrWhiteSpace(f3))
            {
                result.Add(f3);
            }
            return result;
        }

        /// <summary>
        /// Get SelectedFiles from Sp URL like this sp://65.254.46.34:8003/Shared Documents/2010 Compound_comments Doc v1.doc/2.0
        /// </summary>
        /// <param name="spUrl"></param>
        /// <param name="treeViewPath"></param>
        /// <returns></returns>
        private SelectedFiles ConstructSelectedFileFromSpUrl(string spUrl, string treeViewPath)
        {
            spUrl = ConvertUrl(spUrl);
            var urlArray = spUrl.Split('/');
            var result = new SelectedFiles();

            if (spUrl == string.Empty)
            {
                result.Result = new Result()
                {
                    Status = ResultStatus.None,
                    Message = ConvertUrl(spUrl)
                };
                
            }
            else
            {
                var sites =
                    SiteRepository.GetEnableSites().Where(s => spUrl.ToLower().Trim().Contains(s.Url.ToLower().Trim()));

                var site = sites.OrderByDescending(s => s.Url.Length).FirstOrDefault();

                if (site != null)
                {
                    var subSite =
                        site.SubSites?.Where(s => spUrl.ToLower().Trim().Contains(s.Url.ToLower().Trim()))
                            .OrderByDescending(s => s.Url.Length)
                            .FirstOrDefault();
                    if (subSite != null && subSite.Url.Length > site.Url.Length)
                    {
                        site = subSite;
                    }


                    result.Result = new Result() { Status = ResultStatus.Success };
                    var versionLable = urlArray[urlArray.Length - 1];
                    int v3;
                    try
                    {
                        string v1;
                        string v2;
                        if (!versionLable.Contains("."))
                        {
                            v1 = versionLable;
                            v2 = "0";
                        }
                        else
                        {
                            v1 = versionLable.Split('.')[0];
                            v2 = versionLable.Split('.')[1];
                        }
                        v3 = int.Parse(v1) * 512 + int.Parse(v2);
                    }
                    catch (Exception)
                    {
                        throw new Exception("The downloading file URL is invalid");
                        // throw;
                    }

                    result.Files = new List<SelectedFile>()
                    {
                        
                        new SelectedFile()
                        {
                            LocalId = site.Id,
                            UiVersionLabel = urlArray[urlArray.Length - 1],
                            Name = urlArray[urlArray.Length - 2],
                            IsCurrentVersion = !spUrl.Contains("/_vti_history/"),
                            // RelativeUrl = 
                            Url =
                                !spUrl.Contains("/_vti_history/")
                                    ? (spUrl.Replace("/" + urlArray[urlArray.Length - 1], ""))
                                    : spUrl.Replace("/_vti_history/" + v3, "").Replace("/" + urlArray[urlArray.Length - 1], ""),
                            FileVersionUrl =
                                spUrl.Contains("/_vti_history/")
                                    ? (spUrl.Replace("/" + urlArray[urlArray.Length - 1], ""))
                                    : string.Empty,
                            SiteUrl = site.Url,
                            TreeViewPath = treeViewPath
                        }
                    };
                }
                else
                {
                    result.Result = new Result()
                    {
                        Status = ResultStatus.Error,
                        Message = "Can not download the file at " + ConvertUrl(spUrl)
                    };
                }
            }
            return result;
        }

        private string ConvertUrl(string url)
        {
            if (url == null) {
                url = string.Empty;
            }

            if (url.StartsWith("ss://"))
            {
                url = url.Replace("ss://", "https://");
            }
            if (url.StartsWith("sp://"))
            {
                url = url.Replace("sp://", "http://");
            }
            return url;
        }
    }
}
