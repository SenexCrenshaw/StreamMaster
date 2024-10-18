namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SMChannelStreamRankRequest(int SMChannelId, string SMStreamId, int Rank);

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SMChannelChannelRankRequest(int ParentSMChannelId, int ChildSMChannelId, int Rank);
