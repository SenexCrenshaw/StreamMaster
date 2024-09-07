
namespace StreamMaster.Domain.Dto;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class NameLogo
{
    public NameLogo() { }
    public NameLogo(SMChannel smChannel)
    {
        Id = smChannel.Id;
        Name = smChannel.Name;
        Logo = smChannel.Logo;
    }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
}