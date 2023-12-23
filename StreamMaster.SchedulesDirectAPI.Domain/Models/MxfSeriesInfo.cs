using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class MxfSeriesInfo
{
    public string SeriesId { get; }

    private DateTime _seriesStartDate = DateTime.MinValue;
    private DateTime _seriesEndDate = DateTime.MinValue;
    private int _index;
    private string _uid;
    private string _guideImage;
    [XmlIgnore] public string ProtoTypicalProgram;
    [XmlIgnore] public MxfGuideImage mxfGuideImage;

    [XmlIgnore] public Dictionary<string, dynamic> extras = [];

    public MxfSeriesInfo(int index, string seriesId, string? protoTypicalProgram = null)
    {
        _index = index;
        SeriesId = seriesId;
        ProtoTypicalProgram = protoTypicalProgram;
    }
    private MxfSeriesInfo() { }

    /// <summary>
    /// An ID that is unique to the document and defines this element.
    /// Use IDs such as si1, si2, and si3. SeriesInfo is referenced by the Program and Season elements.
    /// </summary>
    [XmlAttribute("id")]
    public string Id
    {
        get => $"si{_index}";
        set { _index = int.Parse(value[2..]); }
    }

    /// <summary>
    /// A unique ID that will remain consistent between multiple versions of this document.
    /// This value starts with "!Series!". Using the format "!Series!seriesName" is somewhat unique, but won't handle cases such as a remake of a series (for example, Knight Rider or Battlestar Galactica).
    /// </summary>
    [XmlAttribute("uid")]
    public string Uid
    {
        get => _uid ?? $"!Series!{SeriesId}";
        set { _uid = value; }
    }

    /// <summary>
    /// The title name of the series.
    /// The maximum length is 512 characters.
    /// </summary>
    [XmlAttribute("title")]
    public string Title { get; set; }

    /// <summary>
    /// A shorter form of the title attribute (if available).
    /// The maximum length is 512 characters. If this value is not available, use the same value as the title attribute.
    /// </summary>
    [XmlAttribute("shortTitle")]
    public string ShortTitle { get; set; }

    /// <summary>
    /// A description of the series.
    /// The maximum length is 512 characters.
    /// </summary>
    [XmlAttribute("description")]
    public string Description { get; set; }

    /// <summary>
    /// A shorter form of the description attribute, if available.
    /// The maximum length is 512 characters. If this value is not available, use the same value as the description attribute.
    /// </summary>
    [XmlAttribute("shortDescription")]
    public string ShortDescription { get; set; }

    /// <summary>
    /// The date the series was first aired.
    /// </summary>
    [XmlAttribute("startAirdate")]
    public string? StartAirdate
    {
        get => _seriesStartDate != DateTime.MinValue ? _seriesStartDate.ToString("yyyy-MM-dd") : null;
        set => _ = DateTime.TryParse(value, out _seriesStartDate);
    }

    /// <summary>
    /// 
    /// </summary>
    [XmlAttribute("endAirdate")]
    public string? EndAirdate
    {
        get => _seriesEndDate != DateTime.MinValue ? _seriesEndDate.ToString("yyyy-MM-dd") : null;
        set => _ = DateTime.TryParse(value, out _seriesEndDate);
    }

    /// <summary>
    /// An image to show for the series.
    /// This value contains the GuideImage id attribute. 
    /// </summary>
    [XmlAttribute("guideImage")]
    public string GuideImage
    {
        get => _guideImage ?? mxfGuideImage?.Id ?? "";
        set { _guideImage = value; }
    }

    /// <summary>
    /// The name of the studio that created this season.
    /// The maximum length is 512 characters.
    /// </summary>
    [XmlAttribute("studio")]
    public string Studio { get; set; }
}
