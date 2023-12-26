using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Person")]
    public ConcurrentDictionary<string, MxfPerson> People { get; set; } = [];

    public MxfPerson FindOrCreatePerson(string name)
    {
        return People.FindOrCreate(name, key => new MxfPerson(People.Count + 1, key));
    }
}
