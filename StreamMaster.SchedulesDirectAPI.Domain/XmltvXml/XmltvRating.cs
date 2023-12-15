using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

public class XmltvRating
{
    private string _value;
    private string _elementValue;

    [XmlText]
    public string Value
    {
        get => _elementValue ?? _value;
        set => _value = value;
    }

    [XmlElement("value")]
    public string ElementValue
    {
        get => null; // We don't want to serialize this if it's being used for deserialization
        set => _elementValue = value;
    }

    [XmlElement("icon")]
    public List<XmltvIcon> Icons { get; set; }

    [XmlAttribute("system")]
    public string System { get; set; }
}