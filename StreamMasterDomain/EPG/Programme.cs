using System.Globalization;
using System.Xml.Serialization;

namespace StreamMasterDomain.EPG;

[XmlRoot(ElementName = "programme")]
public class Programme
{
    private string start = string.Empty;

    private string stop = string.Empty;

    [XmlElement(ElementName = "audio")]
    public TvAudio Audio { get; set; } = new();

    [XmlElement(ElementName = "category")]
    public List<TvCategory> Category { get; set; } = new();

    [XmlAttribute(AttributeName = "channel")]
    public string Channel { get; set; } = string.Empty;


    [XmlIgnore]
    public string ChannelName { get; set; }
    [XmlIgnore]
    public string DisplayName { get; set; }

    [XmlElement(ElementName = "credits")]
    public TvCredits Credits { get; set; } = new();

    [XmlElement(ElementName = "desc")]
    public TvDesc Desc { get; set; } = new();

    [XmlIgnore]
    public int EPGFileId { get; set; }

    [XmlElement(ElementName = "episode-num")]
    public List<TvEpisodenum> Episodenum { get; set; } = new();

    [XmlElement(ElementName = "icon")]
    public List<TvIcon> Icon { get; set; } = new();

    [XmlElement(ElementName = "language")]
    public string Language { get; set; } = string.Empty;

    [XmlElement(ElementName = "new")]
    public string? New { get; set; }

    public bool ShouldSerializeLive()
    {
        return !string.IsNullOrEmpty(Live);
    }

    public bool ShouldSerializeNew()
    {
        return !string.IsNullOrEmpty(New);
    }

    public bool ShouldSerializePremiere()
    {
        return !string.IsNullOrEmpty(Premiere);
    }

    public bool ShouldSerializePreviouslyshown()
    {
        return Previouslyshown != null;
    }

    [XmlElement(ElementName = "live")]
    public string? Live { get; set; }

    [XmlElement(ElementName = "premiere")]
    public string? Premiere { get; set; }

    [XmlElement(ElementName = "previously-shown")]
    public TvPreviouslyshown? Previouslyshown { get; set; }

    [XmlElement(ElementName = "rating")]
    public List<TvRating> Rating { get; set; } = new();

    [XmlAttribute(AttributeName = "start")]
    public string Start
    {
        get => start;
        set
        {
            start = value;
            try
            {
                _ = DateTime.TryParseExact(value, "yyyyMMddHHmmss K", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out DateTime dateValue);
                StartDateTime = dateValue;
            }
            catch
            {
            }
        }
    }

    [XmlIgnore]
    public DateTime StartDateTime { get; set; }

    [XmlAttribute(AttributeName = "stop")]
    public string Stop
    {
        get => stop;
        set
        {
            stop = value;
            try
            {
                _ = DateTime.TryParseExact(value, "yyyyMMddHHmmss K", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out DateTime dateValue);
                StopDateTime = dateValue;
            }
            catch
            {
            }
        }
    }

    [XmlIgnore]
    public DateTime StopDateTime { get; set; }

    [XmlElement(ElementName = "sub-title")]
    public TvSubtitle Subtitle { get; set; } = new();

    [XmlElement(ElementName = "title")]
    public List<TvTitle> Title { get; set; } = new();

    [XmlElement(ElementName = "video")]
    public TvVideo Video { get; set; } = new();
    public string Name { get; set; }
}
