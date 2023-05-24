using System.Xml.Serialization;

namespace StreamMasterDomain.Entities.EPG;

[XmlRoot(ElementName = "actor")]
public class TvActor
{
    [XmlAttribute(AttributeName = "role")]
    public string? Role { get; set; }

    [XmlText]
    public string? Text { get; set; }
}

[XmlRoot(ElementName = "audio")]
public class TvAudio
{
    [XmlElement(ElementName = "stereo")]
    public string? Stereo { get; set; }
}

[XmlRoot(ElementName = "category")]
public class TvCategory
{
    [XmlAttribute(AttributeName = "lang")]
    public string? Lang { get; set; }

    [XmlText]
    public string? Text { get; set; }
}

[XmlRoot(ElementName = "channel")]
public class TvChannel
{
    [XmlElement(ElementName = "display-name")]
    public List<string>? Displayname { get; set; }

    [XmlElement(ElementName = "icon")]
    public TvIcon? Icon { get; set; }

    [XmlAttribute(AttributeName = "id")]
    public string? Id { get; set; }
}

[XmlRoot(ElementName = "credits")]
public class TvCredits
{
    [XmlElement(ElementName = "actor")]
    public List<TvActor>? Actor { get; set; }

    [XmlElement(ElementName = "director")]
    public List<string>? Director { get; set; }

    [XmlElement(ElementName = "producer")]
    public List<string>? Producer { get; set; }

    [XmlElement(ElementName = "writer")]
    public List<string>? Writer { get; set; }
}

[XmlRoot(ElementName = "desc")]
public class TvDesc
{
    [XmlAttribute(AttributeName = "lang")]
    public string? Lang { get; set; }

    [XmlText]
    public string? Text { get; set; }
}

[XmlRoot(ElementName = "episode-num")]
public class TvEpisodenum
{
    [XmlAttribute(AttributeName = "system")]
    public string? System { get; set; }

    [XmlText]
    public string? Text { get; set; }
}

[XmlRoot(ElementName = "icon")]
public class TvIcon
{
    [XmlAttribute(AttributeName = "height")]
    public string? Height { get; set; }

    [XmlAttribute(AttributeName = "src")]
    public string? Src { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public string? Width { get; set; }
}

[XmlRoot(ElementName = "previously-shown")]
public class TvPreviouslyshown
{
    [XmlAttribute(AttributeName = "start")]
    public string? Start { get; set; }
}

[XmlRoot(ElementName = "rating")]
public class TvRating
{
    [XmlAttribute(AttributeName = "system")]
    public string? System { get; set; }

    [XmlElement(ElementName = "value")]
    public string? Value { get; set; }
}

[XmlRoot(ElementName = "sub-title")]
public class TvSubtitle
{
    [XmlAttribute(AttributeName = "lang")]
    public string? Lang { get; set; }

    [XmlText]
    public string? Text { get; set; }
}

[XmlRoot(ElementName = "title")]
public class TvTitle
{
    [XmlAttribute(AttributeName = "lang")]
    public string? Lang { get; set; }

    [XmlText]
    public string? Text { get; set; }
}

[XmlRoot(ElementName = "video")]
public class TvVideo
{
    [XmlElement(ElementName = "quality")]
    public string? Quality { get; set; }
}

[XmlRoot(ElementName = "tv")]
public class Tv
{
    [XmlElement(ElementName = "channel")]
    public List<TvChannel> Channel { get; set; } = new();

    [XmlAttribute(AttributeName = "guide2go")]
    public string Guide2go { get; set; } = string.Empty;

    [XmlElement(ElementName = "programme")]
    public List<Programme> Programme { get; set; } = new();

    [XmlAttribute(AttributeName = "source-info-name")]
    public string Sourceinfoname { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "generator-info-name")]
    public string Generatorinfoname { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "source-info-url")]
    public string Sourceinfourl { get; set; } = string.Empty;
}
