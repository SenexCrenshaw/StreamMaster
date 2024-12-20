namespace StreamMaster.Streams.Channels
{
    /// <inheritdoc/>
    public sealed class ChannelManager(ILogger<ChannelManager> logger, ISourceBroadcasterService streamBroadcasterService, IChannelService channelService, IMessageService messageService
    ) : IChannelManager
    {
        private readonly SemaphoreSlim _registerSemaphore = new(1, 1);
        private readonly Lock _disposeLock = new();
        private bool _disposed = false;
        /// <inheritdoc/>
        public async Task<bool> AddClientToChannelAsync(IClientConfiguration clientConfiguration, int streamGroupProfileId, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Client {UniqueRequestId} requesting channel {Name}.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Exiting GetChannelStreamAsync {UniqueRequestId} {Name} due to cancellation.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);
                    return false;
                }

                IChannelBroadcaster? channelBroadcaster = await channelService.GetOrCreateChannelBroadcasterAsync(clientConfiguration, streamGroupProfileId);

                if (channelBroadcaster == null)
                {
                    await RemoveClientAsync(clientConfiguration);
                    logger.LogInformation("Exiting GetChannelStreamAsync:  {UniqueRequestId} channel {Name} status is null.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);
                    return false;
                }

                await channelBroadcaster.AddChannelStreamerAsync(clientConfiguration);

                await messageService.SendInfo($"Client started streaming {clientConfiguration.SMChannel.Name}", "Client Start");
                return true;
            }
            finally
            {
                //logger.LogInformation("Client {UniqueRequestId} requesting channel {Name} release.", clientConfiguration.UniqueRequestId, clientConfiguration.SMChannel.Name);

                //_ = _registerSemaphore.Release();
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

                if (!await channelService.SwitchChannelToNextStreamAsync(channelStatus, clientConfiguration: null, newSMStreamId))
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
            IClientConfiguration? config = channelService.GetClientStreamerConfiguration(uniqueRequestId);
            if (config != null)
            {
                await RemoveClientAsync(config).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveClientAsync(IClientConfiguration config)
        {
            logger.LogInformation("UnRegister client: {UniqueRequestId} {name}", config.UniqueRequestId, config.SMChannel.Name);
            await channelService.UnRegisterClientAsync(config.UniqueRequestId);
        }

        /// <inheritdoc/>
        public void StopChannel(int smChannelId)
        {
            channelService.StopChannel(smChannelId);
        }

        /// <inheritdoc/>
        public async Task MoveToNextStreamAsync(int smChannelId)
        {
            IChannelBroadcaster? channelStatus = channelService.GetChannelBroadcaster(smChannelId);
            if (channelStatus is null || channelStatus.SMStreamInfo is null)
            {
                logger.LogWarning("Channel not found: {smChannelId}", smChannelId);
                return;
            }

            ISourceBroadcaster? sourceBroadcaster = streamBroadcasterService.GetStreamBroadcaster(channelStatus.SMStreamInfo.Url);

            if (sourceBroadcaster is not null)
            {
                await sourceBroadcaster.StopAsync();
                logger.LogInformation("Simulating stream failure for: {VideoStreamName}", channelStatus.SMStreamInfo.Name);
            }
            else
            {
                logger.LogWarning("Stream not found, cannot simulate stream failure: {smChannelId}", smChannelId);
            }
        }

        /// <inheritdoc/>
        public async Task CancelAllChannelsAsync()
        {
            foreach (ISourceBroadcaster sourceBroadcaster in streamBroadcasterService.GetStreamBroadcasters())
            {
                await sourceBroadcaster.StopAsync();
            }
        }
    }
}