using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class Season
{
    public string SeriesId => mxfSeriesInfo.SeriesId;

    private int _index;
    private string _uid;
    private string _guideImage;
    private string _series;
    private string _title;

    [XmlIgnore] public string ProtoTypicalProgram;
    [XmlIgnore] public MxfGuideImage mxfGuideImage;
    [XmlIgnore] public SeriesInfo mxfSeriesInfo;
    [XmlIgnore] public bool HideSeasonTitle;

    [XmlIgnore] public Dictionary<string, dynamic> extras = [];

    public Season(int index, SeriesInfo seriesInfo, int seasonNumber, string protoTypicalProgram)
    {
        _index = index;
        mxfSeriesInfo = seriesInfo;
        SeasonNumber = seasonNumber;
        ProtoTypicalProgram = protoTypicalProgram;
    }
    private Season() { }

    /// <summary>
    /// An ID that is unique to the document and defines this element.
    /// Use IDs such as sn1, sn2, and sn3. Seasons are referenced by Program elements.
    /// </summary>
    [XmlAttribute("id")]
    public string Id
    {
        get => $"sn{_index}";
        set => _index = int.Parse(value[2..]);
    }

    /// <summary>
    /// A unique ID that will remain consistent between multiple versions of this document.
    /// This uid should start with "!Season!".
    /// </summary>
    [XmlAttribute("uid")]
    public string Uid
    {
        get => _uid ?? $"!Season!{mxfSeriesInfo.SeriesId}_{SeasonNumber}";
        set => _uid = value;
    }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("endAirdate")]
    public string EndAirdate { get; set; }

    /// <summary>
    /// An image to display for this season.
    /// This value contains the GuideImage id attribute.
    /// </summary>
    [XmlAttribute("guideImage")]
    public string GuideImage
    {
        get => _guideImage ?? mxfGuideImage?.Id ?? "";
        set => _guideImage = value;
    }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("seasonNumber")]
    [DefaultValue(0)]
    public int SeasonNumber { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("startAirdate")]
    public string StartAirdate { get; set; }

    /// <summary>
    /// The series ID to which this season belongs.
    /// This value should be specified because it doesn't make sense for a season to not be part of a series.
    /// </summary>
    [XmlAttribute("series")]
    public string Series
    {
        get => _series ?? mxfSeriesInfo.Id;
        set => _series = value;
    }

    /// <summary>
    /// The name of this season.
    /// The maximum length is 512 characters.
    /// </summary>
    [XmlAttribute("title")]
    public string Title
    {
        get => _title ?? (!HideSeasonTitle ? $"{mxfSeriesInfo.Title}, Season {SeasonNumber}" : "");
        set => _title = value;
    }

    /// <summary>
    /// The name of the studio that created this season.
    /// The maximum length is 512 characters.
    /// </summary>
    [XmlAttribute("studio")]
    public string Studio { get; set; }

    /// <summary>
    /// The year this season was aired.
    /// </summary>
    [XmlAttribute("year")]
    public string Year { get; set; }
}
