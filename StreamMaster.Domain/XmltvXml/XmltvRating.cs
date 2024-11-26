using System.Xml.Serialization;

namespace StreamMaster.Domain.XmltvXml;

public class XmltvRating
{
    private string? _value;

    [XmlText]
    public string? Value
    {
        get => ElementValue ?? _value;
        set => _value = value;
    }

    [XmlElement("value")]
    public string? ElementValue { get; set; }

    [XmlElement("icon")]
    public List<XmltvIcon>? Icons { get; set; }

    [XmlAttribute("system")]
    public string? System { get; set; } = string.Empty;
}