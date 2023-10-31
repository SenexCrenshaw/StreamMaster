using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class IdName()
{
    public IdName() { }
    public IdName(string Id, string Name)
    {
        this.Id = Id;
        this.Name = Name;
    }
    public string Id { get; set; }
    public string Name { get; set; }

}