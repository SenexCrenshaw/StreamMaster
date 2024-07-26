using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class IdNameUrl
{
    public required string Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Url { get; set; } = string.Empty;
    public bool IsCustomStream { get; set; }
}