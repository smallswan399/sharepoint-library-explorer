using System.IO;
using System.Xml.Serialization;

namespace Entities
{
    public class SelectedFileVersion
    {
        public int LocalId { get; set; }
        public string SiteUrl { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string VersionLabel { get; set; }
        public string OriginalFileName { get; set; }

        [XmlIgnore]
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