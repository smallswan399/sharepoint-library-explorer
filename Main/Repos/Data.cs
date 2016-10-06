using System.Collections.Generic;
using Entities;

namespace Main.Repos
{
    public class Data
    {
        public List<Site> Sites { get; set; }
        public SelectedFiles SelectedFiles { get; set; }
        public SelectedFileVersions SelectedFileVersions { get; set; }
        public DownloadedFiles DownloadedFiles { get; set; }
        public UploadingFiles UploadingFiles { get; set; }
        public UploadedFiles UploadedFiles { get; set; }
        public string DownloadDirectory { get; set; }

        public Data()
        {
            Sites = new List<Site>();
            SelectedFiles = new SelectedFiles();
            DownloadedFiles = new DownloadedFiles();
            UploadingFiles = new UploadingFiles();
            UploadedFiles = new UploadedFiles();
            SelectedFileVersions = new SelectedFileVersions();
        }
    }
}