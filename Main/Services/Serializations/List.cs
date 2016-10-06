using System;
using System.Xml.Serialization;

namespace Main.Services.Serializations
{
    [Serializable()]
    [XmlRoot(ElementName = "List")]
    public class List
    {
        [XmlAttribute("Title")]
        public string Title { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

        [XmlAttribute("ID")]
        public Guid ID { get; set; }

        [XmlAttribute("Hidden")]
        public string Hidden { get; set; }

        [XmlAttribute("IsApplicationList")]
        public string IsApplicationList { get; set; }

        [XmlAttribute("WebFullUrl")]
        public string WebFullUrl { get; set; }

        [XmlAttribute("BaseType")]
        public string BaseType { get; set; }
        [XmlAttribute("EnableVersioning")]
        public string EnableVersioning { get; set; }
        [XmlAttribute("RequireCheckout")]
        public string RequireCheckout { get; set; }

        [XmlAttribute("EnableMinorVersion")]
        public string EnableMinorVersion { get; set; }
    }
}
