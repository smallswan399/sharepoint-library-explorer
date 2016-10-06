using System;
using System.Collections.Generic;

namespace Main.Services.Domains
{
    public class SharePointLibrary : SharePointObject, ISharePointSiteItem, ISharePointExpandableObjcect, ISharePointFileContainer
    {
        /// <summary>
        /// Can contain many ISharePointLibraryItem
        /// </summary>
        public ICollection<ISharePointLibraryItem> SharePointLibraryItems { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ListName { get; set; }
        /// <summary>
        /// The Library is enable versioning?
        /// </summary>
        public bool EnableVersioning { get; set; }

        public bool EnableMinorVersions { get; set; }
        /// <summary>
        /// Require documents to be checked out before they can be edited?
        /// </summary>
        public bool RequireCheckout { get; set; }
        public string GetSharePointSiteUrl()
        {
            return ParentSharePointSite.Url;
        }

        public string GetSharePointSiteRootUrl()
        {
            return ParentSharePointSite.RootUrl;
        }

        public Guid GetSharePointSiteLocalId()
        {
            return ParentSharePointSite.LocalId;
        }

        public SharePointLibrary()
        {
            SharePointLibraryItems = new List<ISharePointLibraryItem>();
        }

        public SharePointSite ParentSharePointSite { get; set; }
        public Guid Id { get; set; }
        public SharePointLibrary GetAncestorSharePointLibrary()
        {
            return this;
        }

        public override string User
        {
            get { return ParentSharePointSite.User; }
        }
    }
}
