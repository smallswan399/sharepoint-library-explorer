using System;

namespace Entities
{
    //public class SharePointFile
    //{
    //    public int LocalId { get; set; }
    //    public string SiteUrl { get; set; }
    //    public string Url { get; set; }
    //    public string Name { get; set; }
    //    public string LocalPath { get; set; }
    //    public bool Downloaded { get; set; }

    //    public SharePointFile()
    //    {
    //        LocalPath = string.Empty;
    //    }
    //}

    public class UploadedFile
    {
         public Guid LocalId { get; set; }
        public string SiteUrl { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public bool Uploaded { get; set; }

        public UploadedFile()
        {
            LocalPath = string.Empty;
        }
    }
}
