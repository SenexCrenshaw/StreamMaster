namespace StreamMasterDomain.Models;

public class JobStatus
{
    public DateTime LastRun { get; set; } = DateTime.MinValue;
    public DateTime LastSuccessful { get; set; } = DateTime.MinValue;
    public DateTime LastError { get; set; } = DateTime.MinValue;
    public bool IsErrored => LastError > LastRun;
    public bool ForceNextRun { get; set; }
    public bool IsRunning { get; set; }
    public bool Extra { get; set; }
    public override string ToString()
    {
        return $"LastRun: {LastRun}, LastSuccessful: {LastSuccessful}, LastError: {LastError}, IsErrored: {IsErrored}, ForceNextRun: {ForceNextRun}";
    }
}
