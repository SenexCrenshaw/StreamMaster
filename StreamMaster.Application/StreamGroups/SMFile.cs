namespace StreamMaster.Application.StreamGroups;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SGFS(string Name, string Url, List<SMFile> SMFS);

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record SMFile(string Name, string Url);
