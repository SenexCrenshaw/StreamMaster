namespace StreamMaster.Streams.Channels
{
    /// <inheritdoc/>
    public sealed class ChannelManager(
        ILogger<ChannelManager> logger,
        IStreamBroadcasterService streamBroadcasterService,
        IChannelService channelService,
        IMessageService messageService
    ) : IChannelManager
    {
        private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
        private readonly object _disposeLock = new();
        private bool _disposed = false;
        /// <inheritdoc/>
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

                IChannelBroadcaster? channelStatus = await channelService.GetOrCreateChannelBroadcasterAsync(config, streamGroupProfileId);
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

        /// <inheritdoc/>
        public async Task ChangeVideoStreamChannelAsync(string playingSMStreamId, string newSMStreamId, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Starting ChangeVideoStreamChannel with playingSMStreamId: {playingSMStreamId} and newSMStreamId: {newSMStreamId}", playingSMStreamId, newSMStreamId);

            foreach (IChannelBroadcaster channelStatus in channelService.GetChannelStatusesFromSMStreamId(playingSMStreamId))
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
        /// <inheritdoc/>
        public async Task CancelClientAsync(string uniqueRequestId)
        {
            IClientConfiguration? config = channelService.GetClientConfiguration(uniqueRequestId);
            if (config != null)
            {
                await UnRegisterWithChannelManagerAsync(config).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task CancelChannelAsync(int smChannelId)
        {
            IChannelBroadcaster? channelStatus = channelService.GetChannelBroadcaster(smChannelId);
            if (channelStatus is null)
            {
                logger.LogWarning("Channel not found: {smChannelId}", smChannelId);
                return;
            }

            await channelService.CloseChannelAsync(channelStatus, true);
        }

        /// <inheritdoc/>
        public void MoveToNextStream(int smChannelId)
        {
            IChannelBroadcaster? channelStatus = channelService.GetChannelBroadcaster(smChannelId);
            if (channelStatus is null || channelStatus.SMStreamInfo is null)
            {
                logger.LogWarning("Channel not found: {smChannelId}", smChannelId);
                return;
            }

            IStreamBroadcaster? channelDistributor = streamBroadcasterService.GetStreamBroadcaster(channelStatus.SMStreamInfo.Url);

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

        /// <inheritdoc/>
        public void CancelAllChannels()
        {
            foreach (IStreamBroadcaster s in streamBroadcasterService.GetStreamBroadcasters())
            {
                s.Stop();
            }
        }
    }
}