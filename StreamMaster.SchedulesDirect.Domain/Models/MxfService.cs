using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;
public class MxfService
{
    public string StationId { get; set; } = string.Empty;

    private int _index;
    private string? _affiliate;

    public MxfAffiliate? mxfAffiliate;
    public XmltvIcon? XmltvIcon;
    public MxfScheduleEntries MxfScheduleEntries = new();

    [XmlIgnore] public ConcurrentDictionary<string, dynamic> extras = [];
    [XmlIgnore] public int EPGNumber;
    public MxfService(int index, string stationId)
    {
        _index = index;
        StationId = stationId;
        MxfScheduleEntries = new MxfScheduleEntries() { Service = Id, ScheduleEntry = [] };
    }

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
    public string Uid => $"!Service!{StationId}";

    /// <summary>
    /// The display name of the service.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The call sign of the service.
    /// For example, "BBC1".
    /// </summary>
    public string CallSign { get; set; } = string.Empty;

    /// <summary>
    /// The ID of an Affiliate element that to which this service is affiliated.
    /// </summary>
    public string? Affiliate
    {
        get => _affiliate ?? mxfAffiliate?.Uid;
        set => _affiliate = value;
    }

    public int ChNo { get; set; }
}
