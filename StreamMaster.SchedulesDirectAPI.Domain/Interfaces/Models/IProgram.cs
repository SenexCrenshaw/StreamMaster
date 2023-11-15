namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IProgram
    {
        DateTime AirDateTime { get; set; }
        List<string> AudioProperties { get; set; }
        int Duration { get; set; }
        bool? Educational { get; set; }
        string IsPremiereOrFinale { get; set; }
        string LiveTapeDelay { get; set; }
        string Md5 { get; set; }
        bool? New { get; set; }
        bool? Premiere { get; set; }
        string ProgramID { get; set; }
        List<string> VideoProperties { get; set; }
    }
}