namespace StreamMaster.Domain.Configuration;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SDSettingsRequest
{
    public string? PreferredLogoStyle { get; set; }
    public string? AlternateLogoStyle { get; set; }
    //public bool? SeriesPosterArt { get; set; }
    //public bool? SeriesWsArt { get; set; }
    public string? SeriesPosterAspect { get; set; }
    public string? ArtworkSize { get; set; }
    public bool? ExcludeCastAndCrew { get; set; }
    public bool? AlternateSEFormat { get; set; }
    public bool? PrefixEpisodeDescription { get; set; }
    public bool? PrefixEpisodeTitle { get; set; }
    public bool? AppendEpisodeDesc { get; set; }
    public int? SDEPGDays { get; set; }
    public bool? SDEnabled { get; set; }
    public string? SDUserName { get; set; }
    public string? SDCountry { get; set; }
    public string? SDPassword { get; set; }
    public string? SDPostalCode { get; set; }

    [TsProperty(ForceNullable = true)]
    public List<HeadendToView>? HeadendsToView { get; set; }
    [TsProperty(ForceNullable = true)]
    public List<StationIdLineup>? SDStationIds { get; set; }

    public bool? SeasonImages { get; set; }
    public bool? SportsImages { get; set; }

    public bool? SeriesImages { get; set; }
    public bool? XmltvAddFillerData { get; set; }
    //public string? XmltvFillerProgramDescription { get; set; }
    public int? XmltvFillerProgramLength { get; set; }
    public int? MaxSubscribedLineups { get; set; }
    public bool? XmltvIncludeChannelNumbers { get; set; }
    public bool? XmltvExtendedInfoInTitleDescriptions { get; set; }
    public bool? XmltvSingleImage { get; set; }
}
