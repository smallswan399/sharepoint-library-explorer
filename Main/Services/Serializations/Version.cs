using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Main.Services.Serializations
{
    [XmlRoot(ElementName = "result")]
    public class Version
    {
        [XmlAttribute("comments")]
        public string CheckInComment { get; set; }

        [XmlAttribute("created")]
        public DateTime Created { get; set; }

        [XmlAttribute("createdBy")]
        public string CreatedBy { get; set; }

        //[XmlAttribute("Title")]
        //public int ID { get; set; }

        [XmlAttribute("size")]
        public int Size { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("version")]
        public string VersionLabel { get; set; }
    }
}
