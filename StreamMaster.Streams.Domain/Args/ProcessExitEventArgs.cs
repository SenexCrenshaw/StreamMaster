namespace StreamMaster.Streams.Domain.Args;

public class ProcessExitEventArgs : EventArgs
{
    public int ExitCode { get; set; }
}
