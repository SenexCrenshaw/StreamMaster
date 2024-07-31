using StreamMaster.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IChannelDistributorService
    {
        event AsyncEventHandler<ChannelDirectorStopped> OnStoppedEvent;
        IChannelDistributor? GetStreamHandler(string? key);
        IDictionary<string, IStreamHandlerMetrics> GetAggregatedMetrics();
        IChannelDistributor? GetChannelDistributor(string? key);
        Task<IChannelDistributor?> GetOrCreateSourceChannelDistributorAsync(SMStreamInfo smStreamInfo, CancellationToken cancellationToken);
        bool StopAndUnRegister(string key);
        List<IChannelDistributor> GetChannelDistributors();
        Task<IChannelDistributor?> CreateChannelDistributorFromSMChannelDtoAsync(SMChannelDto smChannel, CancellationToken cancellationToken);
    }
}