using System;

namespace Main.Services.Domains
{
    public interface ISharePointExpandableObjcect : ISharePointObject
    {
        string Title { get; set; }
        string Description { get; set; }

        /// <summary>
        /// Get Url of the current site
        /// </summary>
        /// <returns></returns>
        string GetSharePointSiteUrl();

        /// <summary>
        /// Get RootUrl of the current site
        /// </summary>
        /// <returns></returns>
        string GetSharePointSiteRootUrl();
        Guid GetSharePointSiteLocalId();
    }
}
