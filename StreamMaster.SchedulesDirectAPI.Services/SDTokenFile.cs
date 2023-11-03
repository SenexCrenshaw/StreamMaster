namespace StreamMaster.SchedulesDirectAPI;

internal class SDTokenFile
{
    public string? Token { get; set; }
    public DateTime TokenDateTime { get; set; }
    public DateTime LockOutTokenDateTime { get; set; }
}
