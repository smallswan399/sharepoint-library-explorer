using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core;
using Entities;
using Main.Services.Domains;
using Services;

namespace Main.Services
{
    public interface ISharePointService
    {
        SiteSoapClientMapper SiteSoapClientMapper { get; }
        ClearTextCredential Credential { get; }
        SharePointServerVersion SharePointServerVersion { get; }
        TryConnectResult TestConnection();


        SharePointSite GetSharePointSite();
        Task<SharePointSite> GetSharePointSiteAsync();

        /// <summary>
        /// Get Sub sites of current site
        /// </summary>
        /// <returns></returns>
        IEnumerable<SharePointSite> GetSubSharePointSites();

        /// <summary>
        /// Get Sub sites of current site
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SharePointSite>> GetSubSharePointSitesAsync();

        /// <summary>
        /// Get Library of current site
        /// </summary>
        /// <returns></returns>
        IEnumerable<SharePointLibrary> GetSharePointLibraries();

        /// <summary>
        /// Get Library of current site
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SharePointLibrary>> GetSharePointLibrariesAsync();

        SharePointLibrary GetSharePointLibrary(string title);

        /// <summary>
        /// Get all Folder in a Library by Library title
        /// </summary>
        /// <param name="title">Title of parent Library</param>
        /// <returns></returns>
        IEnumerable<SharePointFolder> GetSharePointFolders(string title);


        /// <summary>
        /// Get all Folder in a Library by Library title
        /// </summary>
        /// <param name="title">Title of parent Library</param>
        /// <returns></returns>
        Task<IEnumerable<SharePointFolder>> GetSharePointFolderAsync(string title);

        ///// <summary>
        ///// Get all Folder in a Library by Library Id
        ///// </summary>
        ///// <param name="id">Library Id</param>
        ///// <returns></returns>
        //IEnumerable<SharePointFolder> GetSharePointFolder(Guid id);

        ///// <summary>
        ///// Get sub folders of an folder by folder Id
        ///// </summary>
        ///// <param name="folderId"></param>
        ///// <param name="listId"></param>
        ///// <returns></returns>
        //IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, Guid listId);

        ///// <summary>
        ///// Get sub folders of an folder by folder Id
        ///// </summary>
        ///// <param name="folderId"></param>
        ///// <param name="listTitle"></param>
        ///// <returns></returns>
        //IEnumerable<SharePointFolder> GetSharePointSubFolder(int folderId, string listTitle);

        /// <summary>
        /// Get Folders in a Folder by relative url of parent folder
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        IEnumerable<SharePointFolder> GetSharePointSubFolder(string relativeUrl);


        /// <summary>
        /// Get Folders in a Folder by relative url of parent folder
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        Task<IEnumerable<SharePointFolder>> GetSharePointSubFolderAsync(string relativeUrl);

        /// <summary>
        /// Get file in a library by relativeUrl of library
        /// </summary>
        /// <param name="listTitle"></param>
        /// <returns></returns>
        IEnumerable<ISharePointFile> GetSharePointFiles(string listTitle);

        /// <summary>
        /// Get file in a library by relativeUrl of library
        /// </summary>
        /// <param name="listTitle"></param>
        /// <returns></returns>
        Task<IEnumerable<ISharePointFile>> GetSharePointFilesAsync(string listTitle);

        /// <summary>
        /// Get Files in a folder by relativeUrl of parent Folder
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        IEnumerable<ISharePointFile> GetSharePointFilesInFolder(string relativeUrl);

        /// <summary>
        /// Get Files in a folder by relativeUrl of parent Folder
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        Task<IEnumerable<ISharePointFile>> GetSharePointFilesInFolderAsync(string relativeUrl);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="relativeUrl"></param>
        ///// <returns></returns>
        //Stream DownloadFile(string relativeUrl);

        Stream DownloadFileByUrl(string url);

        Task<Stream> DownloadFileByUrlAsync(string url);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentRelativeUrl"></param>
        /// <param name="name"></param>
        /// <param name="streamFile"></param>
        List<CopyActionResult> UploadFile(string parentRelativeUrl, string name, Stream streamFile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentRelativeUrl"></param>
        /// <param name="name"></param>
        /// <param name="streamFile"></param>
        Task<List<CopyActionResult>> UploadFileAsync(string parentRelativeUrl, string name, Stream streamFile);

        List<CopyActionResult> UploadFile(string absoluteUrl, Stream streamFile);

        /// <summary>
        /// Check exist file in a forder or a list
        /// </summary>
        /// <param name="parentRelativeUrl">Relative Url or parent folder or list</param>
        /// <param name="name">File name</param>
        /// <returns></returns>
        bool CheckFileExist(string parentRelativeUrl, string name);

        //void RefreshCredential();

        IEnumerable<SharePointFileVersion> GetVersions(string url, string relativeUrl);

        Task<IEnumerable<SharePointFileVersion>> GetVersionsAsync(string url, string relativeUrl);

        //IEnumerable<SharePointFileVersion> GetVersions(string url, string listId, string listItemId);

        /// <summary>
        /// Download a history version
        /// </summary>
        /// <param name="url">Absolute url</param>
        /// <returns></returns>
        Stream DownloadHistoryVersion(string url);

        /// <summary>
        /// Allows documents to be checked in to a SharePoint document library remotely.
        /// </summary>
        /// <param name="url">A string that contains the full path to the document to check in.</param>
        /// <param name="checkoutToLocal"></param>
        /// <param name="lastmodified"></param>
        /// <returns></returns>
        bool CheckOutFile(string url, string checkoutToLocal, string lastmodified);

        /// <summary>
        /// Allows documents to be checked in to a SharePoint document library remotely.
        /// </summary>
        /// <param name="url">A string that contains the full path to the document to check in.</param>
        /// <param name="checkoutToLocal"></param>
        /// <param name="lastmodified"></param>
        /// <returns></returns>
        Task<bool> CheckOutFileAsync(string url, string checkoutToLocal, string lastmodified);

        /// <summary>
        /// Allows documents to be checked in to a SharePoint document library remotely.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="comment">A string containing optional check-in comments.</param>
        /// <param name="checkinType">A string representation of the values 0, 1 or 2, where 0 = MinorCheckIn, 1 = MajorCheckIn, and 2 = OverwriteCheckIn.</param>
        /// <returns></returns>
        bool CheckInFile(string url, string comment, string checkinType);

        /// <summary>
        /// Allows documents to be checked in to a SharePoint document library remotely.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="comment">A string containing optional check-in comments.</param>
        /// <param name="checkinType">A string representation of the values 0, 1 or 2, where 0 = MinorCheckIn, 1 = MajorCheckIn, and 2 = OverwriteCheckIn.</param>
        /// <returns></returns>
        Task<bool> CheckInFileAsync(string url, string comment, string checkinType);

        /// <summary>
        /// Get user name of current user (this is domain\alias)
        /// </summary>
        /// <returns></returns>
        string GetCurrentUser();

        /// <summary>
        /// Get user name of current user (this is domain\alias)
        /// </summary>
        /// <returns></returns>
        Task<string> GetCurrentUserAsync();

        SharePointFile GetSharePointFile(string url);

        SharePointLibrary GetParentSharePointLibraryByFileUrl(string fileUrl);
    }
}
