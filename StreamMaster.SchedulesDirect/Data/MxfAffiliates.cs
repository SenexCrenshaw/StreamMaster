using StreamMaster.Domain.Extensions;

using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Affiliate")]
    public ConcurrentDictionary<string, MxfAffiliate> Affiliates { get; set; } = [];

    public MxfAffiliate FindOrCreateAffiliate(string affiliateName)
    {
        return Affiliates.FindOrCreate(affiliateName, key => new MxfAffiliate(affiliateName));
    }
}
