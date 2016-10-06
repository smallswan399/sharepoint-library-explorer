using System.Collections.Generic;
using Core.Libs;
using Entities;
using Main.Libs;
using Microsoft.Win32;

namespace Main.Repos
{
    public class RegistryRepoProvider : IRepoProvider
    {
        // HKEY_CURRENT_USER\Software\Litera2\SharePoint\ + "a sub fix"
        private readonly string rootKey;
        // Registry key store path of the folder that contain the downloaded files
        private const string DownloadDirectoryKeyName = "DownloadDirectory";
        // Registry contain SelectedFiles data
        private const string SelectedFilesKeyName = "SelectedFiles";
        // Registry contain SelectedFileVersions data
        private const string SelectedFileVersionsKeyName = "SelectedFileVersions";
        // Registry contain DownloadedFiles data
        private const string DownloadedFilesKeyName = "DownloadedFiles";
        // Registry contain UploaddingFiles data, this contain local path of the files will be uploaded to the server
        private const string UploaddingFilesKeyName = "UploaddingFiles";
        // Registry contain UploadedFiles data
        private const string UploadedFilesKeyName = "UploadedFiles";
        // Registry contain Sites data
        private const string SitesKeyName = "Sites";

        private readonly Data data;

        public RegistryRepoProvider(string rootKey)
        {
            this.rootKey = rootKey;

            data = InitDataFromRegistry();
            SiteRepository = new SiteRepository(data);
        }

        public ISiteRepository SiteRepository { get; private set; }
        public SelectedFiles SelectedFiles
        {
            get { return data.SelectedFiles; }
            set { data.SelectedFiles = value; }
        }

        public DownloadedFiles DownloadedFiles
        {
            get { return data.DownloadedFiles; }
            set { data.DownloadedFiles = value; }
        }

        public UploadingFiles UploadingFiles
        {
            get { return data.UploadingFiles; }
            set { data.UploadingFiles = value; }
        }
        public UploadedFiles UploadedFiles
        {
            get { return data.UploadedFiles; }
            set { data.UploadedFiles = value; }
        }

        public SelectedFileVersions SelectedFileVersions
        {
            get { return data.SelectedFileVersions; }
            set { data.SelectedFileVersions = value; }
        }

        public string DownloadDirectory
        {
            get { return data.DownloadDirectory; }
            set { data.DownloadDirectory = value; }
        }

        public int SaveChanges(ResultMode mode = ResultMode.None)
        {
            // save sites to the registry
            Registry.SetValue(rootKey, SitesKeyName, data.Sites.XmlSerializeToString());
            // save selected files
            Registry.SetValue(rootKey, SelectedFilesKeyName, data.SelectedFiles.XmlSerializeToString());
            // save downloaded files
            Registry.SetValue(rootKey, DownloadedFilesKeyName, data.DownloadedFiles.XmlSerializeToString());
            // save uploading files
            Registry.SetValue(rootKey, UploaddingFilesKeyName, data.UploadingFiles.XmlSerializeToString());
            // save uploaded files
            Registry.SetValue(rootKey, UploadedFilesKeyName, data.UploadedFiles.XmlSerializeToString());
            // save selected file versions
            Registry.SetValue(rootKey, SelectedFileVersionsKeyName, data.SelectedFileVersions.XmlSerializeToString());
            // save download directory
            Registry.SetValue(rootKey, DownloadDirectoryKeyName, data.DownloadDirectory);

            return 0;
            //throw new NotImplementedException();
        }

        //public void SaveSelectedFilesResult()
        //{
        //    if (data.SelectedFiles.Result.Status == ResultStatus.Success)
        //    {
        //        Registry.SetValue(rootKey, SelectedFilesKeyNameKeyNameOld,
        //               ListStringToString(data.SelectedFiles.Files.Select(s => s.Url).ToList()), RegistryValueKind.String);
        //        Registry.SetValue(rootKey, Result, 0, RegistryValueKind.DWord);
        //    }
        //    else
        //    {
        //        Registry.SetValue(rootKey, Result, -11, RegistryValueKind.DWord);
        //    }
        //}

        //public void SaveDownloadedFilesResult()
        //{
        //    if (data.DownloadedFiles.Result.Status == ResultStatus.Success)
        //    {
        //        Registry.SetValue(rootKey, DownloadedFilesKeyNameOld,
        //               ListStringToString(data.DownloadedFiles.Files.Select(s => s.Url).ToList()), RegistryValueKind.String);
        //        Registry.SetValue(rootKey, Result, 0, RegistryValueKind.DWord);
        //    }
        //    else
        //    {
        //        Registry.SetValue(rootKey, Result, -11, RegistryValueKind.DWord);
        //    }
        //}

        //public void SaveUploadResult()
        //{
            
        //}

        /// <summary>
        /// Create new Data object from registry
        /// </summary>
        /// <returns></returns>
        private Data InitDataFromRegistry()
        {
            // get SelectedFiles from registry
            var selectedFilesString = (string)Registry.GetValue(rootKey, SelectedFilesKeyName, string.Empty);
            var selectedFileVersionsString = (string)Registry.GetValue(rootKey, SelectedFileVersionsKeyName, string.Empty);
            var downloadedFilesString = (string) Registry.GetValue(rootKey, DownloadedFilesKeyName, string.Empty);

            var uploaddingFilesString = (string)Registry.GetValue(rootKey, UploaddingFilesKeyName, string.Empty);

            var uploadedFilesString = (string)Registry.GetValue(rootKey, UploadedFilesKeyName, string.Empty);
            var downloadDirectoryString = (string)Registry.GetValue(rootKey, DownloadDirectoryKeyName, string.Empty);
            var sitesString = (string)Registry.GetValue(rootKey, SitesKeyName, string.Empty);

            return new Data()
            {
                DownloadDirectory =
                    string.IsNullOrWhiteSpace(downloadDirectoryString) ? string.Empty : downloadDirectoryString,
                DownloadedFiles =
                    string.IsNullOrWhiteSpace(downloadedFilesString)
                        ? new DownloadedFiles()
                        : downloadedFilesString.XmlDeserializeFromString<DownloadedFiles>(),
                SelectedFiles =
                    string.IsNullOrWhiteSpace(selectedFilesString)
                        ? new SelectedFiles()
                        : selectedFilesString.XmlDeserializeFromString<SelectedFiles>(),
                SelectedFileVersions =
                    string.IsNullOrWhiteSpace(selectedFileVersionsString)
                        ? new SelectedFileVersions()
                        : selectedFileVersionsString.XmlDeserializeFromString<SelectedFileVersions>(),
                Sites =
                    string.IsNullOrWhiteSpace(sitesString)
                        ? new List<Site>()
                        : sitesString.XmlDeserializeFromString<List<Site>>(),
                UploadingFiles =
                    string.IsNullOrWhiteSpace(uploaddingFilesString)
                        ? new UploadingFiles()
                        : uploaddingFilesString.XmlDeserializeFromString<UploadingFiles>(),
                UploadedFiles =
                    string.IsNullOrWhiteSpace(uploadedFilesString)
                        ? new UploadedFiles()
                        : uploadedFilesString.XmlDeserializeFromString<UploadedFiles>()
            };
        }
    }
}