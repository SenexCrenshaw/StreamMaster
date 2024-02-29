namespace StreamMaster.Domain.Models;

public class ProcessExitEventArgs : EventArgs
{
    public int ExitCode { get; set; }
}
