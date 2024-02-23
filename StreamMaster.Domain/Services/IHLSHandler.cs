namespace StreamMaster.Domain.Services
{
    public interface IHLSHandler
    {
        string Name { get; }
        string Url { get; }

        void Stop();
    }
}