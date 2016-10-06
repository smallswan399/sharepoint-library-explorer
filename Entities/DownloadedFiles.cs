using System.Collections.Generic;

namespace Entities
{
    public class DownloadedFiles
    {
        public List<DownloadedFile> Files { get; set; }
        public Result Result { get; set; }

        public DownloadedFiles()
        {
            Result = new Result();
            Files = new List<DownloadedFile>();
        }
    }
}