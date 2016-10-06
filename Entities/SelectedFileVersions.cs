using System.Collections.Generic;

namespace Entities
{
    public class SelectedFileVersions
    {
        public List<SelectedFileVersion> Files { get; set; }
        public Result Result { get; set; }

        public SelectedFileVersions()
        {
            Result = new Result();
            Files = new List<SelectedFileVersion>();
        }
    }
}