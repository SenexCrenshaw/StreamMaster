namespace signlar_function_builder;

public class EndPoint
{
    public string DTO { get; set; } = string.Empty;

    public string DTOArray => DTO.EndsWith("[]") ? DTO : DTO + "[]";
    public string DTONoArray => DTO.EndsWith("[]") ? DTO[..DTO.IndexOf("[]")] : DTO;

    public string Getter { get; set; } = string.Empty;
    public string HubBroadCast { get; set; } = string.Empty;
    public string IndexBy { get; set; } = "id";
    public bool IsSingle { get; internal set; }
    public bool JustUpdates { get; internal set; } = false;
    public string NS { get; set; } = string.Empty;
    public string SortBy { get; set; } = string.Empty;
}
