namespace Entities
{
    public class UploadingFile
    {
        /// <summary>
        /// File's local path
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// Remote file in an uploading as new version session
        /// Is null or empty if the app upload file as new profile
        /// </summary>
        // public string RemoteFile { get; set; }
        public SelectedFile VersionHolderSelectedFileFile { get; set; }
    }
}