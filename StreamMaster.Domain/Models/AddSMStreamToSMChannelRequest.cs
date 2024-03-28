using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Models;


//public record SMStreamSMChannelRequest(int SMChannelId, string SMStreamId) { }
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SMChannelRankRequest(int SMChannelId, string SMStreamId, int Rank) { }

//public record SMChannelLogoRequest(int SMChannelId, string logo) { }