using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class IdNameUrl
{
    public IdNameUrl() { }

    public IdNameUrl(string Id, string Name, string Url)
    {
        this.Id = Id;
        this.Name = Name;
        this.Url = Url;
    }
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}