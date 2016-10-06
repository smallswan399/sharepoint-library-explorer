using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Main.Services.Domains
{
    /// <summary>
    /// A file container. This can be a SharePointFolder or SharePointLibrary
    /// </summary>
    public interface ISharePointFileContainer
    {
        /// <summary>
        /// Get Ancestor SharePointLibrary that this container is belong to
        /// </summary>
        /// <returns></returns>
        SharePointLibrary GetAncestorSharePointLibrary();

        /// <summary>
        /// Contain a list of ISharePointLibraryItem. An ISharePointFileContainer can contain a list of SharePointFile or SharePointFolder
        /// </summary>
        ICollection<ISharePointLibraryItem> SharePointLibraryItems { get; set; }
    }
}
