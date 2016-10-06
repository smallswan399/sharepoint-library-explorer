using System.Collections.Generic;

namespace Entities
{
    public class SelectedFiles
    {
        public List<SelectedFile> Files { get; set; }
        public Result Result { get; set; }

        public SelectedFiles()
        {
            Result = new Result();
            Files = new List<SelectedFile>();
        }
    }
}