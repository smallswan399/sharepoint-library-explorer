using System;

namespace Main.Services.Domains
{
    /// <summary>
    /// This can be a SharePointFolder or SharePointFile
    /// </summary>
    public interface ISharePointLibraryItem : ISharePointObject
    {
        SharePointLibrary ParentSharePointLibrary { get; set; }
        SharePointFolder ParentSharePointFolder { get; set; }
        long FileSize { get; set; }
        DateTime LastModifiedDateTime { get; set; }
        DateTime CreatedDateTime { get; set; }
        string Name { get; set; }
        string Author { get; set; }
        int Id { get; set; }
        Guid Guid { get; set; }
        string GetSharePointSiteUrl();
        Guid GetSharePointSiteLocalId();
        string GetSharePointSiteRootUrl();
        Guid GetParentLibraryId();
    }
}
