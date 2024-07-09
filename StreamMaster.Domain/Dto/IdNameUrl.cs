using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class IdNameUrl
{
    public IdNameUrl() { }

    public IdNameUrl(int Id, string Name, string Url)
    {
        this.Id = Id;
        this.Name = Name;
        this.Url = Url;
    }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}