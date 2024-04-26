using Reinforced.Typings.Attributes;

namespace StreamMaster.SchedulesDirect.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationChannelName
{

    public string Id => Channel;

    public string Channel { get; set; }

    public string ChannelName { get; set; }

    public string DisplayName { get; set; }
}
