namespace StreamMasterDomain.Repository;

public class ProgrammeChannel
{

    public string Channel { get; set; } = string.Empty;
    public DateTime EndDateTime { get; set; }
    public int EPGFileId { get; set; }
    public int ProgrammeCount { get; set; }
    public DateTime StartDateTime { get; set; }
}
