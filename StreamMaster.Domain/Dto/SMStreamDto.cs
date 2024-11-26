
namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class SMStreamDto : SMStream, IMapFrom<SMStream>
{
    public int Rank { get; set; }
    public string RealUrl { get; set; } = string.Empty;
    public List<LogoInfo > ChannelMembership { get; set; } = [];
}
