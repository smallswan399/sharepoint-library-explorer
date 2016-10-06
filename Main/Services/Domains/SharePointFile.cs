using System;
using System.Collections.Generic;

namespace Main.Services.Domains
{
    public class SharePointFile : SharePointObject, ISharePointFile
    {
        public string Author { get; set; }
        public string FileExtension { get; set; }

        /// <summary>
        /// Null if the SharePointFile is nested in a SharePointFolder
        /// </summary>
        public SharePointLibrary ParentSharePointLibrary { get; set; }
        /// <summary>
        /// Null if the SharePointFile is nested in a SharePointLibrary
        /// </summary>
        public SharePointFolder ParentSharePointFolder { get; set; }
        public long FileSize { get; set; }
        public bool HadFilledFileVersion { get; set; }
        public string CheckoutUser { get; set; }
        public void Download()
        {
            throw new NotImplementedException();
        }

        public SharePointLibrary GetAncestorSharePointLibrary()
        {
            return ParentSharePointLibrary ?? ParentSharePointFolder.GetAncestorSharePointLibrary();
        }

        public DateTime LastModifiedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string UIVersionLabel { get; set; }
        public string GetSharePointSiteUrl()
        {
            if (ParentSharePointLibrary != null)
            {
                return ParentSharePointLibrary.ParentSharePointSite.Url;
            }
            return ParentSharePointFolder.GetSharePointSiteUrl();
        }

        public Guid GetSharePointSiteLocalId()
        {
            if (ParentSharePointLibrary != null)
            {
                return ParentSharePointLibrary.ParentSharePointSite.LocalId;
            }
            return ParentSharePointFolder.GetSharePointSiteLocalId();
        }

        public string GetSharePointSiteRootUrl()
        {
            if (ParentSharePointLibrary != null)
            {
                return ParentSharePointLibrary.ParentSharePointSite.RootUrl;
            }
            return ParentSharePointFolder.GetSharePointSiteRootUrl();
        }

        public Guid GetParentLibraryId()
        {
            return ParentSharePointLibrary != null ? ParentSharePointLibrary.Id : ParentSharePointFolder.GetParentLibraryId();
        }

        public IEnumerable<SharePointFileVersion> SharePointFileVersions { get; set; }

        public override string User
        {
            get { return ParentSharePointLibrary == null ? ParentSharePointFolder.User : ParentSharePointLibrary.User; }
        }

        /// <summary>
        /// Can check out the file if no one checked out the file
        /// </summary>
        public bool CanCheckOut
        {
            get { return string.IsNullOrWhiteSpace(CheckoutUser); }
        }
    }
}
