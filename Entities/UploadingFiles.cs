using System.Collections.Generic;

namespace Entities
{
    public class UploadingFiles
    {
        public List<UploadingFile> Files { get; set; }
        public Result Result { get; set; }
        public UploadingFiles()
        {
            Files = new List<UploadingFile>();
            Result = new Result();
        }
    }
}