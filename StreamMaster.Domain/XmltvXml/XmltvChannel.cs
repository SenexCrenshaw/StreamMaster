using System.Xml.Serialization;

namespace StreamMaster.Domain.XmltvXml
{
    //
    public class XmltvChannel
    {
        public XmltvChannel()
        { }

        public XmltvChannel(string id)
        {
            Id = id;
        }

        public XmltvChannel(string id, List<XmltvText> displayNames, List<XmltvIcon> icons)
        {
            Id = id;
            DisplayNames = displayNames;
            Icons = icons;
        }

        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;

        [XmlElement("display-name")]
        public List<XmltvText>? DisplayNames { get; set; }

        //[XmlElement("lcn")]
        //public List<XmltvText>? Lcn { get; set; }

        [XmlElement("icon")]
        public List<XmltvIcon>? Icons { get; set; }

        //[XmlElement("url")]
        //public List<string>? Urls { get; set; }
    }
}