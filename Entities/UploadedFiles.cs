using System.Collections.Generic;

namespace Entities
{
    public class UploadedFiles
    {
        public List<UploadedFile> Files { get; set; }
        public Result Result { get; set; }

        public UploadedFiles()
        {
            Result = new Result();
            Files = new List<UploadedFile>();
        }
    }
}