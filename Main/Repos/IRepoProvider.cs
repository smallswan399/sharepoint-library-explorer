using System;
using Entities;

namespace Main.Repos
{
    public interface IRepoProvider
    {
        ISiteRepository SiteRepository { get; }
        SelectedFiles SelectedFiles { get; set; }
        DownloadedFiles DownloadedFiles { get; set; }
        UploadingFiles UploadingFiles { get; set; }
        UploadedFiles UploadedFiles { get; set; }
        SelectedFileVersions SelectedFileVersions { get; set; }
        string DownloadDirectory { get; set; }
        int SaveChanges(ResultMode mode = ResultMode.None);
    }
}
