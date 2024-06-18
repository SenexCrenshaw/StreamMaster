namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true)]
public class StreamGroupProfile
{
    public int Id { get; set; }
    public int StreamGroupId { get; set; }
    public string Name { get; set; }
    public string FileProfileName { get; set; }
    public string VideoProfileName { get; set; }
}


