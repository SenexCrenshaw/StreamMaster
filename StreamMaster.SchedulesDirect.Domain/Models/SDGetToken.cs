namespace StreamMaster.SchedulesDirect.Domain.Models;

public class SDGetToken
{
    public int Code { get; set; }
    public DateTime Datetime { get; set; }
    public string? Message { get; set; }
    public string? ServerID { get; set; }
    public string? Token { get; set; }
}
