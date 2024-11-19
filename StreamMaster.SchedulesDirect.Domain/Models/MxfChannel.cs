using System.ComponentModel;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Domain.Models;

public class MxfChannel
{
    private readonly MxfLineup? _mxfLineup;
    private readonly MxfService? _mxfService;

    private string? _uid;
    private string? _lineup;
    private string? _service;
    private int? _number;

    public MxfChannel(MxfLineup lineup, MxfService service, int number = -1, int subnumber = 0)
    {
        _mxfLineup = lineup;
        _mxfService = service;
        _number = number;
        SubNumber = subnumber;
        MatchName = service.CallSign;
    }
    private MxfChannel() { }

    /// <summary>
    /// A unique ID that is consistent between loads. 
    /// This value should take the form "!Channel!uniqueLineupName!number_subNumber", where uniqueLineupName is the Lineup element's uid, and number and subNumber are the values from the number and subNumber attribute.
    /// </summary>
    [XmlAttribute("uid")]
    public string? Uid
    {
        get => _uid ?? $"!Channel!{_mxfLineup?.LineupId}!{_mxfService?.StationId}_{_number}_{SubNumber}";
        set => _uid = value;
    }

    /// <summary>
    /// A reference to the Lineup element.
    /// This value should always be "l1".
    /// </summary>
    [XmlAttribute("lineup")]
    public string? Lineup
    {
        get => _lineup ?? _mxfLineup?.Id;
        set => _lineup = value;
    }

    /// <summary>
    /// The service to reference.
    /// This value should be the Service id attribute.
    /// </summary>
    [XmlAttribute("service")]
    public string? Service
    {
        get => _service ?? _mxfService?.Id;
        set => _service = value;
    }

    /// <summary>
    /// Used to automatically map this channel of listings onto the scanned channel.
    /// If not specified, the value of the Service element's callSign attribute is used as the default value.
    /// If matchName is specified, it should take the following format according to the signal type:
    /// PAL/NTSC: The call sign
    /// DVB-T: The call sign, or a string of format "DVBT:onid:tsid:sid"
    /// DVB-S: A string of format "DVBS:sat:freq:onid:tsid:sid"
    /// Where: 
    /// onid is the originating network ID.
    /// tsid is the transport stream ID.
    /// sid is the service ID.
    /// sat is the satellite position.
    /// freq is the frequency, in MHz.
    /// Note All of these values are expressed as decimal integer numbers.
    /// </summary>
    [XmlAttribute("matchName")]
    public string? MatchName { get; set; }

    /// <summary>
    /// The number used to access the service.
    /// Do not specify this value if you want Windows Media Center to assign the channel number.
    /// </summary>
    [XmlAttribute("number")]
    [DefaultValue(0)]
    public int? Number
    {
        get => _number <= 0 ? -1 : _number;
        set => _number = value;
    }

    /// <summary>
    /// The subnumber used to access the service.
    /// </summary>
    [XmlAttribute("subNumber")]
    [DefaultValue(0)]
    public int SubNumber { get; set; }
}
