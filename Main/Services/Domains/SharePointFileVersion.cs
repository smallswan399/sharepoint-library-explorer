using System;
using System.IO;

namespace Main.Services.Domains
{
    public class SharePointFileVersion : SharePointObject
    {
        public string CheckInComment { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public int ID { get; set; }
        public long? Size { get; set; }
        /// <summary>
        /// File version's URL
        /// </summary>
        // public string Url { get; set; }
        public string VersionLabel { get; set; }
        /// <summary>
        /// Name of file and all file versions
        /// </summary>
        public string OriginalFileName { get; set; }
        /// <summary>
        /// The current file
        /// </summary>
        public SharePointFile SharePointFile { get; set; }

        /// <summary>
        /// File name will be used to download file version
        /// </summary>
        public string FileNameToSave
        {
            get
            {
                return Path.GetFileNameWithoutExtension(OriginalFileName) + "-v" + VersionLabel +
                       Path.GetExtension(OriginalFileName);
            }
        }
    }
}