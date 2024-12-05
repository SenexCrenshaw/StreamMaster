using StreamMaster.Domain.Extensions;

namespace StreamMaster.Domain.Models;

public class JobStatus
{
    public DateTime LastRun { get; private set; } = DateTime.MinValue;
    public DateTime LastSuccessful { get; private set; } = DateTime.MinValue;
    public DateTime LastError { get; private set; } = DateTime.MinValue;
    public bool IsErrored => LastError > LastRun;
    public bool ForceNextRun { get; private set; }
    public bool IsRunning { get; private set; }
    public bool Extra { get; private set; }
    public override string ToString()
    {
        return $"LastRun: {LastRun}, LastSuccessful: {LastSuccessful}, LastError: {LastError}, IsErrored: {IsErrored}, ForceNextRun: {ForceNextRun}";
    }

    public void SetSuccessful()
    {
        LastRun = SMDT.UtcNow;
        IsRunning = false;
        LastSuccessful = LastRun;
        ForceNextRun = false;
    }

    public void SetError()
    {
        LastRun = SMDT.UtcNow;
        LastError = LastRun;
        IsRunning = false;
        ForceNextRun = false;
    }

    public void SetForceNextRun(bool Extra = false)
    {
        ForceNextRun = true;
        if (Extra)
        {
            this.Extra = Extra;
        }
    }

    public void ClearForce()
    {
        ForceNextRun = false;
        Extra = false;
    }

    public void SetStart()
    {
        IsRunning = true;
    }

    public void SetStop()
    {
        IsRunning = false;
    }
}
