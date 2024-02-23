namespace StreamMaster.Domain.Services
{
    public interface IHLSHandler
    {
        string Name { get; }

        void Stop();
    }
}