using System.Collections.Concurrent;
namespace StreamMaster.Streams.Streams;

public class HLSManager(ILogger<HLSManager> logger, ICryptoService cryptoService, ISwitchToNextStreamService switchToNextStreamService, IStreamTracker streamTracker, ILoggerFactory loggerFactory, IOptionsMonitor<HLSSettings> intHLSSettings, IOptionsMonitor<Setting> intSettings)
    : IHLSManager
{
    private readonly ConcurrentDictionary<int, IM3U8ChannelStatus> channelStatuses = new();
    private readonly ConcurrentDictionary<string, IHLSHandler> hlsHandlers = new();

    private readonly CancellationTokenSource HLSCancellationTokenSource = new();

    public IM3U8ChannelStatus? GetChannelStatus(int smChannelId)
    {
        channelStatuses.TryGetValue(smChannelId, out IM3U8ChannelStatus? channelStatus);
        return channelStatus;
    }

    public async Task<IM3U8ChannelStatus?> TryAddAsync(SMChannelDto smChannel, CancellationToken cancellationToken)
    {
        IM3U8ChannelStatus? channelStatus = GetChannelStatus(smChannel.Id);

        if (channelStatus == null)
        {
            logger.LogInformation("No existing channel for {ChannelVideoStreamId} {name}", smChannel.Id, smChannel.Name);

            channelStatus = new M3U8ChannelStatus(smChannel);

            //if (smChannel.SMChannelType == StreamMaster.Domain.Enums.SMChannelTypeEnum.CustomPlayList)
            //{
            //    channelStatus.CustomPlayList = customPlayListBuilder.GetCustomPlayList(smChannel.Name);
            //    if (channelStatus.CustomPlayList == null || channelStatus.CustomPlayList.CustomStreamNfos.Count == 0)
            //    {
            //        logger.LogError("No custom video files for {name}", smChannel.Name);
            //        return null;
            //    }
            //}

            if (!await switchToNextStreamService.SetNextStreamAsync(channelStatus).ConfigureAwait(false))
            {
                return null;

            }
            channelStatuses.TryAdd(smChannel.Id, channelStatus);
        }

        if (channelStatus.SMStreamInfo == null)
        {
            logger.LogError("channelStatus.SMStreamInfo error");
            channelStatuses.TryRemove(smChannel.Id, out _);
            return null;
        }

        if (!streamTracker.HasStream(channelStatus.SMStreamInfo.Id))
        {
            logger.LogInformation("Adding HLSHandler for {name}", smChannel.Name);
            HLSHandler hlsHandler = new(loggerFactory, cryptoService, channelStatus, intSettings, intHLSSettings);
            hlsHandler.Start();

            HLSSettings hlsSettings = intHLSSettings.CurrentValue;

            hlsHandler.ProcessExited += (sender, args) =>
            {
                //bool didSwitch = await channelService.SetNextChildVideoStream(channelStatus);
                if (!channelStatus.Shutdown)
                {
                    hlsHandler.Start();
                    logger.LogInformation("Switched to next stream for {Name}", smChannel.Name);
                }
                else
                {
                    hlsHandler.Stop();
                    logger.LogInformation("HLSHandler Process Exited for {Name} with exit code {ExitCode}", smChannel.Name, args.ExitCode);
                }
            };

            int timeOut = hlsSettings.HLSM3U8CreationTimeOutInSeconds;
            if (!await FileUtil.WaitForFileAsync(channelStatus.M3U8File, timeOut, 50, cancellationToken).ConfigureAwait(false))
            {
                logger.LogWarning("HLS segment timeout {FileName}, exiting", channelStatus.M3U8File);
                Stop(channelStatus.SMStreamInfo.Id);
                return null;
            }

            string tsFile = Path.Combine(channelStatus.M3U8Directory, "0.ts");
            if (!await FileUtil.WaitForFileAsync(tsFile, timeOut, 50, cancellationToken).ConfigureAwait(false))
            {
                logger.LogWarning("TS segment timeout {FileName}, exiting", tsFile);
                Stop(channelStatus.SMStreamInfo.Id);
                return null;
            }

            hlsHandlers.TryAdd(channelStatus.SMStreamInfo.Id, hlsHandler);
            streamTracker.AddStream(channelStatus.SMStreamInfo.Id);
        }
        else
        {
            logger.LogInformation("Reuse existing stream handler for {smChannel.Id} {name}", smChannel.Id, smChannel.Name);
        }

        if (channelStatus.SMStreamInfo == null || string.IsNullOrEmpty(channelStatus.SMStreamInfo.Url))
        {
            logger.LogError("Exiting SMStreamInfo for {smChannel.Id} {name} empty", smChannel.Id, smChannel.Name);
            return null;
        }

        return channelStatus;
    }

    public IHLSHandler? Get(string smStreamId)
    {
        return hlsHandlers.GetValueOrDefault(smStreamId);
    }

    public void Stop(string smStreamId)
    {
        //accessTracker.RemoveAccessTime(smStreamId);

        IHLSHandler? handler = Get(smStreamId);
        streamTracker.RemoveStream(smStreamId);

        if (handler is not null)
        {
            logger.LogInformation("Stopping HLSHandler for {smStreamId}", smStreamId);
            handler.Stop();

            hlsHandlers.TryRemove(smStreamId, out _);

        }
        HLSCancellationTokenSource.Cancel();
    }

    public void Dispose()
    {
        foreach (IHLSHandler handler in hlsHandlers.Values)
        {
            handler.Stop();
            handler.Dispose();
        }
        hlsHandlers.Clear();
    }
}