using System;

namespace Entities
{
    public class SelectedFile
    {
        public string TreeViewPath { get; set; }
        public Guid LocalId { get; set; }
        public string SiteUrl { get; set; }
        /// <summary>
        /// Selected file's Url
        /// </summary>
        public string Url { get; set; }
        public string RelativeUrl { get; set; }
        public string FileVersionUrl { get; set; }
        public string Name { get; set; }
        public string UiVersionLabel { get; set; }
        public bool IsCurrentVersion { get; set; }
        public string Author { get; set; }
        public string GetSPUrl()
        {
            var url = IsCurrentVersion ? Url : FileVersionUrl;
            if (url.ToLower().StartsWith("https"))
            {
                url = url.Replace("https", "ss");
            }
            if (url.StartsWith("http"))
            {
                url = url.Replace("http", "sp");
            }
            // the url contain a sub fix that is UIVersionLabel. Need to remove this before to get a right file or file version url
            url += "/" + UiVersionLabel;
            return url;
        }
    }
}