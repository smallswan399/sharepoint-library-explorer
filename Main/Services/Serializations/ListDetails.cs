using System;
using System.Xml.Serialization;

namespace Main.Services.Serializations
{
    [Serializable()]
    [XmlRoot(ElementName = "List")]
    public class ListDetails
    {
        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("ID")]
        public Guid ID { get; set; }
        [XmlAttribute("RootFolder")]
        public string RelativeUrl { get; set; }
    }
}