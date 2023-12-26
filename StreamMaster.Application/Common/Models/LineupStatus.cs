namespace StreamMaster.Application.Common.Models;

public class LineupStatus
{
    public int ScanInProgress { get; set; } = 0;
    public int ScanPossible { get; set; } = 0;
    public string Source { get; set; } = "Cable";
    public string[] SourceList { get; set; } = new string[1] { "Cable" };
}