
namespace StreamMaster.Application.SMChannels.Queries;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class IdNameUrl
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public IdNameUrl()
    {
    }
    public IdNameUrl(int Id, string Name, string Url)
    {
        this.Id = Id;
        this.Name = Name;
        this.Url = Url;
    }
}
