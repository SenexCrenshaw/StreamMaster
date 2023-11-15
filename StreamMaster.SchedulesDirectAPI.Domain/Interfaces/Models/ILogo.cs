namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ILogo
    {
        int Height { get; set; }
        string Md5 { get; set; }
        string URL { get; set; }
        int Width { get; set; }
    }
}