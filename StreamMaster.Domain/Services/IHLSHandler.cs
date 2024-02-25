namespace StreamMaster.Domain.Services
{
    public interface IHLSHandler
    {
        event EventHandler<ProcessExitEventArgs> ProcessExited;
        string Name { get; }
        string Url { get; }
        string Id { get; }
        Stream? Stream { get; }
        void Stop();
    }
}