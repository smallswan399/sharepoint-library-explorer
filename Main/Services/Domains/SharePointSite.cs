using System;
using System.Collections.Generic;

namespace Main.Services.Domains
{
    public class SharePointSite : SharePointObject, ISharePointSiteItem, ISharePointExpandableObjcect
    {
        /// <summary>
        /// The id value that stored in the database at the local
        /// </summary>
        public Guid LocalId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Microsoft.SharePoint.Client.TimeZone TimeZone { get; set; }
        public string GetSharePointSiteUrl()
        {
            return Url;
        }

        public string GetSharePointSiteRootUrl()
        {
            return RootUrl;
        }

        public Guid GetSharePointSiteLocalId()
        {
            return LocalId;
        }

        public ICollection<ISharePointSiteItem> SharePointSiteItems { get; set; }
        public SharePointSite()
        {
            SharePointSiteItems = new List<ISharePointSiteItem>();
        }
        public SharePointSite ParentSharePointSite { get; set; }
        public Guid Id { get; set; }
        

        private string user;
        public override string User
        {
            get { return ParentSharePointSite == null ? user : ParentSharePointSite.User; }
            set { user = value; }
        }
    }
}
