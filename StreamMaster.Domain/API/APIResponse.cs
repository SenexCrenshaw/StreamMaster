using Reinforced.Typings.Attributes;

using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.API;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class APIResponse<T>
{
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public bool? IsError { get; set; }
    public PagedResponse<T>? PagedResponse { get; set; }
}
