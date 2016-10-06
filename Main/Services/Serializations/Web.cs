using System;
using System.Xml.Serialization;

namespace Services.Serializations
{
    [Serializable()]
    [XmlRoot(ElementName = "Web")]
    public class Web
    {
        [XmlAttribute("Title")]
        public string Title { get; set; }
        [XmlAttribute("Url")]
        public string Url { get; set; }
        [XmlAttribute("Description")]
        public string Description { get; set; }
    }
}
