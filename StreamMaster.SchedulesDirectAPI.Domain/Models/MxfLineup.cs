using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class MxfLineup
{
    public string LineupId { get; }

    private string _uid;
    private int _index;

    public MxfLineup(int index, string lineupId, string lineupName)
    {
        _index = index;
        LineupId = lineupId;
        Name = lineupName;
    }
    private MxfLineup() { }

    /// <summary>
    /// An ID that is unique to the document and defines this element.
    /// Use the value l1.
    /// </summary>
    [XmlAttribute("id")]
    public string Id
    {
        get => $"l{_index}";
        set { _index = int.Parse(value[1..]); }
    }

    /// <summary>
    /// An ID that uniquely identifies the lineup.
    /// The uid value should be in the form "!Lineup!uniqueLineupName", where uniqueLineupName is a unique ID for this lineup across all Lineup elements.
    /// Lesson learned: uid value should start with !MCLineup! -> this is the way to present information in about guide.
    /// </summary>
    [XmlAttribute("uid")]
    public string Uid
    {
        get => _uid ?? $"!MCLineup!{LineupId}";
        set { _uid = value; }
    }

    /// <summary>
    /// The name of the lineup.
    /// </summary>
    [XmlAttribute("name")]
    public string Name { get; set; }

    /// <summary>
    /// The primary provider.
    /// This value should always be set to "!MCLineup!MainLineup".
    /// </summary>
    [XmlAttribute("primaryProvider")]
    public string PrimaryProvider
    {
        get => _index <= 1 ? "!MCLineup!MainLineup" : null;
        set { }
    }

    [XmlArrayItem("Channel")]
    public List<MxfChannel> channels { get; set; } = [];
}
