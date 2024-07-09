namespace StreamMaster.Domain.Services
{
    public interface IHLSHandler
    {
        event EventHandler<ProcessExitEventArgs> ProcessExited;
        SMChannel SMChannel { get; }
        void Stop();
    }
}