using System;

namespace Services
{
    public class SiteSoapClientMapper
    {
        /// <summary>
        /// Site local Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Site Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// First level node site Url
        /// </summary>
        public string RootUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SharePointRootUrl { get; set; }
    }
}