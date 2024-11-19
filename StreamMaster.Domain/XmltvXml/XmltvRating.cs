using System.Xml.Serialization;

namespace StreamMaster.Domain.XmltvXml;

public class XmltvRating
{
    private string _value = string.Empty;

    [XmlText]
    public string Value
    {
        get => ElementValue ?? _value;
        set => _value = value;
    }

    [XmlElement("value")]
    public string ElementValue { get; set; } = string.Empty;

    [XmlElement("icon")]
    public List<XmltvIcon>? Icons { get; set; }

    [XmlAttribute("system")]
    public string? System { get; set; } = string.Empty;
}