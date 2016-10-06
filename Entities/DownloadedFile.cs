using System;

namespace Entities
{
    public class DownloadedFile
    {
        public Guid LocalId { get; set; }
        public string SiteUrl { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public bool Downloaded { get; set; }

        public DownloadedFile()
        {
            LocalPath = string.Empty;
        }
    }
}