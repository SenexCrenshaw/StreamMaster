using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class MxfAffiliate
{
    private string _uid;
    private string _logoImage;

    public MxfGuideImage mxfGuideImage;

    public MxfAffiliate(string name)
    {
        Name = name;
    }
    private MxfAffiliate() { }

    /// <summary>
    /// The display name of the network.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// An ID that uniquely identifies the affiliate.
    /// This value should take the form "!Affiliate!name", where name is the value of the name attribute.
    /// </summary>

    public string Uid
    {
        get => _uid ?? $"!Affiliate!{Name}";
        set { _uid = value; }
    }

    /// <summary>
    /// Specifies a network logo to display.
    /// This value contains a GuideImage id attribute.
    /// </summary>

    public string LogoImage
    {
        get => _logoImage ?? mxfGuideImage?.Id;
        set { _logoImage = value; }
    }
}
