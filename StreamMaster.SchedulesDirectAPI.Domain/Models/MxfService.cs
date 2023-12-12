namespace StreamMaster.SchedulesDirectAPI.Domain.Models;
public class MxfService
{
    public string StationId { get; set; }

    private int _index;
    private string _affiliate;
    private string _logoImage;
    public string UidOverride;
    public MxfAffiliate mxfAffiliate;
    public MxfGuideImage mxfGuideImage;
    public MxfScheduleEntries MxfScheduleEntries;

    public Dictionary<string, dynamic> extras = [];

    public MxfService(int index, string stationId)
    {
        _index = index;
        StationId = stationId;
        MxfScheduleEntries = new MxfScheduleEntries() { Service = Id, ScheduleEntry = [] };
    }
    private MxfService() { }

    /// <summary>
    /// An ID that is unique to the document and defines this element.
    /// Use IDs such as s1, s2, s3, and so forth.
    /// </summary>

    public string Id
    {
        get => $"s{_index}";
        set => _index = int.Parse(value[1..]);
    }

    /// <summary>
    /// An ID that uniquely identifies the service.
    /// Should be of the form "!Service!name", where name is the value of the name attribute.
    /// </summary>

    public string Uid
    {
        get => UidOverride ?? $"!Service!{StationId}";
        set => UidOverride = value;
    }

    /// <summary>
    /// The display name of the service.
    /// </summary>

    public string Name { get; set; }

    /// <summary>
    /// The call sign of the service.
    /// For example, "BBC1".
    /// </summary>

    public string CallSign { get; set; }

    /// <summary>
    /// The ID of an Affiliate element that to which this service is affiliated.
    /// </summary>

    public string Affiliate
    {
        get => _affiliate ?? mxfAffiliate?.Uid;
        set => _affiliate = value;
    }

    /// <summary>
    /// Specifies a logo image to display.
    /// This value contains a GuideImage id attribute. When searching for a logo to display, the service is searched first, and then its affiliate.
    /// </summary>

    public string LogoImage
    {
        get => _logoImage ?? mxfGuideImage?.Id ?? "";
        set => _logoImage = value;
    }
}
