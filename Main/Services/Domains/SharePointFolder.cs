using System;
using System.Collections.Generic;

namespace Main.Services.Domains
{
    public class SharePointFolder : SharePointObject, ISharePointLibraryItem, ISharePointExpandableObjcect, ISharePointFileContainer
    {
        /// <summary>
        /// SharePointFolder can contain many ISharePointLibraryItem (Folder or File)
        /// </summary>
        public ICollection<ISharePointLibraryItem> SharePointLibraryItems { get; set; }
        public SharePointLibrary ParentSharePointLibrary { get; set; }
        public SharePointFolder ParentSharePointFolder { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int Id { get; set; }
        public Guid Guid { get; set; }

        public string GetSharePointSiteUrl()
        {
            if (ParentSharePointLibrary != null)
            {
                return ParentSharePointLibrary.ParentSharePointSite.Url;
            }
            return ParentSharePointFolder.GetSharePointSiteUrl();
        }

        public string GetSharePointSiteRootUrl()
        {
            if (ParentSharePointLibrary != null)
            {
                return ParentSharePointLibrary.ParentSharePointSite.RootUrl;
            }
            return ParentSharePointFolder.GetSharePointSiteRootUrl();
        }

        public Guid GetSharePointSiteLocalId()
        {
            if (ParentSharePointLibrary != null)
            {
                return ParentSharePointLibrary.ParentSharePointSite.LocalId;
            }
            return ParentSharePointFolder.GetSharePointSiteLocalId();
        }

        public Guid GetParentLibraryId()
        {
            return ParentSharePointLibrary != null ? ParentSharePointLibrary.Id : ParentSharePointFolder.GetParentLibraryId();
        }

        public string Title { get; set; }
        public string Description { get; set; }

        public SharePointFolder()
        {
            SharePointLibraryItems = new List<ISharePointLibraryItem>();
        }

        public SharePointLibrary GetAncestorSharePointLibrary()
        {
            return ParentSharePointLibrary ?? ParentSharePointFolder.GetAncestorSharePointLibrary();
        }

        public override string User
        {
            get { return ParentSharePointLibrary == null ? ParentSharePointFolder.User : ParentSharePointLibrary.User; }
        }
    }
}
