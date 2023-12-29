namespace StreamMaster.Domain.Models;

public class SystemKeyValue
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
