using System.Globalization;
using System.Xml.Serialization;

namespace StreamMasterDomain.Entities.EPG;

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
    public string New { get; set; } = string.Empty;

    [XmlElement(ElementName = "previously-shown")]
    public TvPreviouslyshown Previouslyshown { get; set; } = new();

    [XmlElement(ElementName = "rating")]
    public TvRating Rating { get; set; } = new();

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
    public TvTitle Title { get; set; } = new();

    [XmlElement(ElementName = "video")]
    public TvVideo Video { get; set; } = new();
}
