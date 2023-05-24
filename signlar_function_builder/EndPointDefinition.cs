namespace signlar_function_builder;

public class EndPointDefinition
{
    public string DTO { get; set; } = string.Empty;
    public string DTOArray => DTO.EndsWith("[]") ? DTO : DTO + "[]";
    public string HubBroadCast { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
