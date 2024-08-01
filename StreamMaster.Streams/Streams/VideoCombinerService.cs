using Microsoft.Extensions.DependencyInjection;

using System.Threading.Channels;

namespace StreamMaster.Streams.Streams
{
    public class VideoCombinerService(ILogger<VideoCombinerService> logger, IVideoCombiner videoCombiner, IChannelManager channelManager, IServiceProvider serviceProvider)
        : IVideoCombinerService
    {
        public async Task CombineVideosServiceAsync(IClientConfiguration config, int SMChannelId1, int SMChannelId2, int SMChannelId3, int SMChannelId4, ChannelWriter<byte[]> channelWriter, CancellationToken cancellationToken)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            IRepositoryWrapper repositoryWrapper = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

            SMChannel? smChannel1 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId1);
            if (smChannel1 == null)
            {
                logger.LogError("SMChannel1 {smChannel1} of the channels is not found", smChannel1);
                return;
            }
            SMChannel? smChannel2 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId2);
            if (smChannel1 == null)
            {
                logger.LogError("SMChannel2 {smChannel2} of the channels is not found", smChannel2);
                return;
            }
            SMChannel? smChannel3 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId3);
            if (smChannel1 == null)
            {
                logger.LogError("SMChannel3 {smChannel3} of the channels is not found", smChannel3);
                return;
            }
            SMChannel? smChannel4 = await repositoryWrapper.SMChannel.FirstOrDefaultAsync(a => a.Id == SMChannelId4);
            if (smChannel1 == null)
            {
                logger.LogError("SMChannel4 {smChannel4} of the channels is not found", smChannel4);
                return;
            }

            IClientConfiguration config1 = config.DeepCopy();
            IClientConfiguration config2 = config.DeepCopy();
            IClientConfiguration config3 = config.DeepCopy();
            IClientConfiguration config4 = config.DeepCopy();

            config1.SetUniqueRequestId(config1.UniqueRequestId + "-1");
            config2.SetUniqueRequestId(config1.UniqueRequestId + "-2");
            config3.SetUniqueRequestId(config1.UniqueRequestId + "-3");
            config4.SetUniqueRequestId(config1.UniqueRequestId + "-4");

            Stream? smChannel1Stream = await channelManager.GetChannelStreamAsync(config1, cancellationToken);
            if (smChannel1Stream == null)
            {
                logger.LogError("SMChannel1 {smChannel1} getting stream failed", smChannel1);
                return;
            }
            Stream? smChannel2Stream = await channelManager.GetChannelStreamAsync(config2, cancellationToken);
            if (smChannel2Stream == null)
            {
                logger.LogError("SMChannel2 {smChannel2} getting stream failed", smChannel2);
                return;
            }
            Stream? smChannel3Stream = await channelManager.GetChannelStreamAsync(config3, cancellationToken);
            if (smChannel3Stream == null)
            {
                logger.LogError("SMChannel3 {smChannel3} getting stream failed", smChannel3);
                return;
            }
            Stream? smChannel4Stream = await channelManager.GetChannelStreamAsync(config4, cancellationToken);
            if (smChannel4Stream == null)
            {
                logger.LogError("SMChannel4 {smChannel4} getting stream failed", smChannel4);
                return;
            }

            await videoCombiner.CombineVideosAsync(smChannel1Stream, smChannel2Stream, smChannel3Stream, smChannel4Stream, channelWriter, cancellationToken);

        }
    }
}
