namespace StreamMaster.Streams.Channels
{
    public sealed class ChannelManager(
        ILogger<ChannelManager> logger,
        IChannelBroadcasterService channelDistributorService,
        IChannelService channelService,
        IMessageService messageService
    ) : IChannelManager
    {
        private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
        private readonly object _disposeLock = new();
        private bool _disposed = false;

        public async Task<Stream?> GetChannelStreamAsync(IClientConfiguration config, int streamGroupProfileId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _registerSemaphore.WaitAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Exiting GetChannelStreamAsync due to cancellation.");
                    return null;
                }

                IChannelStatus? channelStatus = await channelService.GetOrCreateChannelStatusAsync(config, streamGroupProfileId);
                if (channelStatus == null)
                {
                    await UnRegisterWithChannelManagerAsync(config);
                    logger.LogInformation("Exiting GetChannelStreamAsync: channel status is null.");
                    return null;
                }

                await messageService.SendInfo($"Client started streaming {config.SMChannel.Name}", "Client Start");
                return config.ClientStream as Stream;
            }
            finally
            {
                _ = _registerSemaphore.Release();
            }
        }

        public async Task ChangeVideoStreamChannelAsync(string playingSMStreamId, string newSMStreamId, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Starting ChangeVideoStreamChannel with playingSMStreamId: {playingSMStreamId} and newSMStreamId: {newSMStreamId}", playingSMStreamId, newSMStreamId);

            foreach (IChannelStatus channelStatus in channelService.GetChannelStatusesFromSMStreamId(playingSMStreamId))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (!await channelService.SwitchChannelToNextStreamAsync(channelStatus, newSMStreamId))
                {
                    logger.LogWarning("Exiting ChangeVideoStreamChannel. Could not change channel to {newSMStreamId}", newSMStreamId);
                    return;
                }

                return;
            }

            logger.LogWarning("Channel not found: {playingSMStreamId}", playingSMStreamId);
            logger.LogDebug("Exiting ChangeVideoStreamChannel due to channel not found.");
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_disposed)
                {
                    return;
                }

                try
                {
                    channelService.Dispose();
                    _registerSemaphore.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during disposal of ChannelManager.");
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        public async Task CancelClientAsync(string uniqueRequestId)
        {
            IClientConfiguration? config = await channelService.GetClientStreamerConfigurationAsync(uniqueRequestId);
            if (config != null)
            {
                await UnRegisterWithChannelManagerAsync(config).ConfigureAwait(false);
            }
        }

        public async Task RemoveClientAsync(IClientConfiguration config)
        {
            logger.LogInformation("Client exited");
            await UnRegisterWithChannelManagerAsync(config);
        }

        private async Task UnRegisterWithChannelManagerAsync(IClientConfiguration config)
        {
            logger.LogInformation("UnRegisterWithChannelManagerAsync client: {UniqueRequestId} {name}", config.UniqueRequestId, config.SMChannel.Name);

            if (!await channelService.UnRegisterClientAsync(config.UniqueRequestId))
            {
                logger.LogWarning("UnRegisterWithChannelManagerAsync: channelService does not have client: {UniqueRequestId} {name}", config.UniqueRequestId, config.SMChannel.Name);
            }
        }

        public async Task CancelChannelAsync(int smChannelId)
        {
            IChannelStatus? channelStatus = channelService.GetChannelStatus(smChannelId);
            if (channelStatus is null)
            {
                logger.LogWarning("Channel not found: {smChannelId}", smChannelId);
                return;
            }

            await channelService.CloseChannelAsync(channelStatus, true);
        }

        public void MoveToNextStream(int smChannelId)
        {
            IChannelStatus? channelStatus = channelService.GetChannelStatus(smChannelId);
            if (channelStatus is null || channelStatus.SMStreamInfo is null)
            {
                logger.LogWarning("Channel not found: {smChannelId}", smChannelId);
                return;
            }

            IChannelBroadcaster? channelDistributor = channelDistributorService.GetChannelBroadcaster(channelStatus.SMStreamInfo.Url);

            if (channelDistributor is not null)
            {
                channelDistributor.Stop();
                logger.LogInformation("Simulating stream failure for: {VideoStreamName}", channelStatus.SMStreamInfo.Name);
            }
            else
            {
                logger.LogWarning("Stream not found, cannot simulate stream failure: {smChannelId}", smChannelId);
            }
        }

        public void CancelAllChannels()
        {
            foreach (IChannelBroadcaster s in channelDistributorService.GetChannelBroadcasters())
            {
                s.Stop();
            }
        }
    }
}