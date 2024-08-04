namespace StreamMaster.Streams;

public sealed class ChannelStatusService(ILogger<IChannelStatus> logger, IVideoInfoService videoInfoService, IChannelDistributorService channelDistributorService, IDubcer Dubcer)
    : IChannelStatusService
{
    public async Task<IChannelStatus> NewChannelStatus(SMChannelDto smChannel)
    {
        IChannelStatus channelStatus = new ChannelStatus(logger, videoInfoService, channelDistributorService,  smChannel);
        IChannelDistributor? channelDistributor = await channelDistributorService.CreateChannelDistributorFromSMChannelDtoAsync(smChannel, channelStatus, CancellationToken.None);
        if (channelDistributor == null)
        {
            throw new ApplicationException("channelDistributor == null");
        }
        channelStatus.ChannelDistributor = channelDistributor;
        return channelStatus;
    }
}