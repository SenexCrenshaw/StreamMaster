namespace StreamMaster.Streams;

public sealed class ChannelStatusService(ILogger<IChannelStatus> logger, IVideoInfoService videoInfoService, IChannelDistributorService channelDistributorService, IDubcer Dubcer)
    : IChannelStatusService
{
    public IChannelStatus NewChannelStatus(SMChannelDto smChannel)
    {
        ChannelStatus channelStatus = new(logger, videoInfoService, channelDistributorService, Dubcer, smChannel);
        IChannelDistributor? channelDistributor = channelDistributorService.CreateChannelDistributorFromSMChannelDtoAsync(smChannel, CancellationToken.None).Result;
        if (channelDistributor == null)
        {
            int a = 1;
        }
        channelStatus.ChannelDistributor = channelDistributor;
        return channelStatus;
    }
}