namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class SDTokenResponse
{
    public string response { get; set; }
    public int code { get; set; }
    public string serverID { get; set; }
    public string message { get; set; }
    public DateTime datetime { get; set; }
}
