using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class MxfProgram
{
    public string ProgramId { get; }

    private string _uid;
    private string _keywords;
    private string _season;
    private string _series;
    private string _guideImage;
    private DateTime _originalAirDate = DateTime.MinValue;

    [XmlIgnore] public string UidOverride;
    [XmlIgnore] public SeriesInfo mxfSeriesInfo;
    [XmlIgnore] public Season mxfSeason;
    [XmlIgnore] public MxfGuideImage? mxfGuideImage;
    [XmlIgnore] public List<MxfKeyword> mxfKeywords = [];
    [XmlIgnore] public bool IsAdultOnly;

    [XmlIgnore] public Dictionary<string, dynamic> extras = [];

    public MxfProgram(int index, string programId)
    {
        Id = index;
        ProgramId = programId;
    }
    public MxfProgram() { }

    /// <summary>
    /// An ID that is unique to the document and defines this element.
    /// This value should be an integer without a letter prefix.
    /// Program elements are referenced by ScheduleEntry elements.
    /// </summary>
    [XmlAttribute("id")]
    [DefaultValue(0)]
    public int Id { get; set; }

    /// <summary>
    /// A unique ID that will remain consistent between multiple versions of this document.
    /// This uid should start with "!Program!".
    /// </summary>
    [XmlAttribute("uid")]
    public string Uid
    {
        get => _uid ?? (!string.IsNullOrEmpty(UidOverride) ? $"!Program!{UidOverride}" : $"!Program!{ProgramId}");
        set => _uid = value;
    }

    /// <summary>
    /// The title of the program (for example, Lost).
    /// The maximum length is 512 characters.
    /// </summary>
    [XmlAttribute("title")]
    public string Title { get; set; }

    /// <summary>
    /// The episode title of the program (for example, The others attack).
    /// The maximum length is 512 characters.
    /// </summary>
    [XmlAttribute("episodeTitle")]
    public string EpisodeTitle { get; set; }

    /// <summary>
    /// The description of this program.
    /// The maximum length is 2048 characters.
    /// </summary>
    [XmlAttribute("description")]
    public string Description { get; set; }

    /// <summary>
    /// A shorter form of the description attribute, if available.
    /// The maximum length is 512 characters. If a short description is not available, do not specify a value.
    /// </summary>
    [XmlAttribute("shortDescription")]
    public string ShortDescription { get; set; }

    /// <summary>
    /// Recording requests only
    /// </summary>
    [XmlAttribute("movieId")]
    public string MovieId { get; set; }

    /// <summary>
    /// The language of the program.
    /// </summary>
    [XmlAttribute("language")]
    public string Language { get; set; }

    /// <summary>
    /// Recording requests only
    /// </summary>
    [XmlAttribute("movieIdLookupHash")]
    public string MovieIdLookupHash { get; set; }

    /// <summary>
    /// The year the program was created.
    /// If unknown, this value is 0.
    /// </summary>
    [XmlAttribute("year")]
    [DefaultValue(0)]
    public int Year { get; set; }

    /// <summary>
    /// The season number of the program (for example, 1).\
    /// If unknown, this value is 0.
    /// </summary>
    [XmlAttribute("seasonNumber")]
    [DefaultValue(0)]
    public int SeasonNumber { get; set; }

    /// <summary>
    /// The episode number of the program in the season.
    /// If unknown, this value is 0.
    /// </summary>
    [XmlAttribute("episodeNumber")]
    [DefaultValue(0)]
    public int EpisodeNumber { get; set; }

    /// <summary>
    /// The original air date (in local time) of the program. Use this value to determine whether this program is a repeat.
    /// </summary>
    [XmlAttribute("originalAirdate")]
    public string? OriginalAirdate
    {
        get => !IsGeneric && _originalAirDate != DateTime.MinValue && extras.ContainsKey("newAirDate")
                ? (string)extras["newAirDate"].ToString("yyyy-MM-dd")
                : _originalAirDate != DateTime.MinValue ? _originalAirDate.ToString("yyyy-MM-dd") : null;
        set => _ = DateTime.TryParse(value, out _originalAirDate);
    }

    [XmlAttribute("wdsTimestamp")]
    public string WdsTimestamp { get; set; }

    /// <summary>
    /// A comma-delimited list of keyword IDs. This value specifies the Keyword attributes that this program has.
    /// </summary>
    [XmlAttribute("keywords")]
    public string Keywords
    {
        get => _keywords ?? string.Join(",", mxfKeywords.Select(k => k.Id).ToArray());
        set => _keywords = value;
    }

    /// <summary>
    /// The ID of the season that this program belongs to, if any.
    /// If this value is not known, do not specify a value.
    /// </summary>
    [XmlAttribute("season")]
    public string Season
    {
        get => _season ?? mxfSeason?.Id;
        set => _season = value;
    }

    /// <summary>
    /// The ID of the series that this program belongs to, if any.
    /// If this value is not known, do not specify a value.
    /// </summary>
    [XmlAttribute("series")]
    public string Series
    {
        get => _series ?? mxfSeriesInfo?.Id;
        set => _series = value;
    }

    /// <summary>
    /// The star rating of the program.
    /// Each star equals two points. For example, a value of "3" is equal to 1.5 stars.
    /// </summary>
    [XmlAttribute("halfStars")]
    [DefaultValue(0)]
    public int HalfStars { get; set; }

    /// <summary>
    /// The MPAA movie rating.
    /// Possible values are:
    /// 0 = Unknown
    /// 1 = G
    /// 2 = PG
    /// 3 = PG13
    /// 4 = R
    /// 5 = NC17
    /// 6 = X
    /// 7 = NR
    /// 8 = AO
    /// </summary>
    [XmlAttribute("mpaaRating")]
    [DefaultValue(0)]
    public int MpaaRating { get; set; }

    /// <summary>
    /// Recording requests only
    /// </summary>
    [XmlAttribute("isBroadbandAvailable")]
    [DefaultValue(false)]
    public bool IsBroadbandAvailable { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isAction")]
    [DefaultValue(false)]
    public bool IsAction { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isComedy")]
    [DefaultValue(false)]
    public bool IsComedy { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isDocumentary")]
    [DefaultValue(false)]
    public bool IsDocumentary { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isDrama")]
    [DefaultValue(false)]
    public bool IsDrama { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isEducational")]
    [DefaultValue(false)]
    public bool IsEducational { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isHorror")]
    [DefaultValue(false)]
    public bool IsHorror { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isIndy")]
    [DefaultValue(false)]
    public bool IsIndy { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isMusic")]
    [DefaultValue(false)]
    public bool IsMusic { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isRomance")]
    [DefaultValue(false)]
    public bool IsRomance { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isScienceFiction")]
    [DefaultValue(false)]
    public bool IsScienceFiction { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isSoap")]
    [DefaultValue(false)]
    public bool IsSoap { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isThriller")]
    [DefaultValue(false)]
    public bool IsThriller { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isProgramEpisodic")]
    [DefaultValue(false)]
    public bool IsProgramEpisodic { get; set; }

    /// <summary>
    /// Indicates whether the program is a movie.
    /// This value determines whether the program appears in the Movies category of the Guide grid and in other movie-related locations.
    /// </summary>
    [XmlAttribute("isMovie")]
    [DefaultValue(false)]
    public bool IsMovie { get; set; }

    /// <summary>
    /// Indicates whether the program is a miniseries.
    /// </summary>
    [XmlAttribute("isMiniseries")]
    [DefaultValue(false)]
    public bool IsMiniseries { get; set; }

    /// <summary>
    /// Indicates whether the program is a limited series.
    /// </summary>
    [XmlAttribute("isLimitedSeries")]
    [DefaultValue(false)]
    public bool IsLimitedSeries { get; set; }

    /// <summary>
    /// Indicates whether the program is paid programming.
    /// </summary>
    [XmlAttribute("isPaidProgramming")]
    [DefaultValue(false)]
    public bool IsPaidProgramming { get; set; }

    /// <summary>
    /// Indicates whether the program is a serial.
    /// </summary>
    [XmlAttribute("isSerial")]
    [DefaultValue(false)]
    public bool IsSerial { get; set; }

    /// <summary>
    /// Indicates whether the program is part of a series.
    /// </summary>
    [XmlAttribute("isSeries")]
    [DefaultValue(false)]
    public bool IsSeries { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isSeasonPremiere")]
    [DefaultValue(false)]
    public bool IsSeasonPremiere { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isSeasonFinale")]
    [DefaultValue(false)]
    public bool IsSeasonFinale { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isSeriesPremiere")]
    [DefaultValue(false)]
    public bool IsSeriesPremiere { get; set; }

    /// <summary>
    /// Undocumented
    /// </summary>
    [XmlAttribute("isSeriesFinale")]
    [DefaultValue(false)]
    public bool IsSeriesFinale { get; set; }

    /// <summary>
    /// Indicates whether the program is a short film.
    /// </summary>
    [XmlAttribute("isShortFilm")]
    [DefaultValue(false)]
    public bool IsShortFilm { get; set; }

    /// <summary>
    /// Indicates whether the program is a special.
    /// This value is used in the Guide's grid view categories.
    /// </summary>
    [XmlAttribute("isSpecial")]
    [DefaultValue(false)]
    public bool IsSpecial { get; set; }

    /// <summary>
    /// Indicates whether the program is a sports program.
    /// This value is used in the Guide's grid view categories.
    /// </summary>
    [XmlAttribute("isSports")]
    [DefaultValue(false)]
    public bool IsSports { get; set; }

    /// <summary>
    /// Indicates whether the program is a news show.
    /// This value is used in the Guide's grid view categories.
    /// </summary>
    [XmlAttribute("isNews")]
    [DefaultValue(false)]
    public bool IsNews { get; set; }

    /// <summary>
    /// Indicates whether the program is for children.
    /// This value is used in the Guide's grid view categories.
    /// </summary>
    [XmlAttribute("isKids")]
    [DefaultValue(false)]
    public bool IsKids { get; set; }

    /// <summary>
    /// Indicates whether the program is a reality show.
    /// </summary>
    [XmlAttribute("isReality")]
    [DefaultValue(false)]
    public bool IsReality { get; set; }

    /// <summary>
    /// Indicates program is soft/contains generic description.
    /// Use for series episodes that are not known yet.
    /// </summary>
    [XmlAttribute("isGeneric")]
    [DefaultValue(false)]
    public bool IsGeneric { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasAdult")]
    [DefaultValue(false)]
    public bool HasAdult { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasBriefNudity")]
    [DefaultValue(false)]
    public bool HasBriefNudity { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasGraphicLanguage")]
    [DefaultValue(false)]
    public bool HasGraphicLanguage { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasGraphicViolence")]
    [DefaultValue(false)]
    public bool HasGraphicViolence { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasLanguage")]
    [DefaultValue(false)]
    public bool HasLanguage { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasMildViolence")]
    [DefaultValue(false)]
    public bool HasMildViolence { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasNudity")]
    [DefaultValue(false)]
    public bool HasNudity { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasRape")]
    [DefaultValue(false)]
    public bool HasRape { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasStrongSexualContent")]
    [DefaultValue(false)]
    public bool HasStrongSexualContent { get; set; }

    /// <summary>
    /// This value indicates the reason for the MPAA rating.
    /// </summary>
    [XmlAttribute("hasViolence")]
    [DefaultValue(false)]
    public bool HasViolence { get; set; }

    /// <summary>
    /// Recording requests only
    /// </summary>
    [XmlAttribute("hasOnDemand")]
    [DefaultValue(false)]
    public bool HasOnDemand { get; set; }

    /// <summary>
    /// This value contains an image to display for the program.
    /// Contains the value of a GuideImage id attribute. When a program is selected in the UI, the Guide searches for an image to display.The search order is first the program, its season, then its series.
    /// </summary>
    [XmlAttribute("guideImage")]
    public string GuideImage
    {
        get => _guideImage ?? mxfGuideImage?.Id ?? "";
        set => _guideImage = value;
    }

    [XmlElement("ActorRole")]
    public List<MxfPersonRank>? ActorRole { get; set; }

    [XmlElement("WriterRole")]
    public List<MxfPersonRank>? WriterRole { get; set; }

    [XmlElement("GuestActorRole")]
    public List<MxfPersonRank>? GuestActorRole { get; set; }

    [XmlElement("HostRole")]
    public List<MxfPersonRank>? HostRole { get; set; }

    [XmlElement("ProducerRole"), XmlIgnore]
    public List<MxfPersonRank>? ProducerRole { get; set; }

    [XmlElement("DirectorRole")]
    public List<MxfPersonRank>? DirectorRole { get; set; }
    public int EPGNumber { get; set; }
}
