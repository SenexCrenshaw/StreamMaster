using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class IdName
{
    public IdName() { }

    public IdName(string Id, string Name)
    {
        this.Id = Id;
        this.Name = Name;
    }
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}