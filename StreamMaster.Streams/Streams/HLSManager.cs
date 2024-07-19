using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public class HLSManager(ILogger<HLSManager> logger, IChannelService channelService, IStreamTracker streamTracker, ILogger<HLSHandler> HLSHandlerlogger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intSettings)
    : IHLSManager
{
    private readonly ConcurrentDictionary<int, IHLSHandler> hlsHandlers = new();

    private readonly CancellationTokenSource HLSCancellationTokenSource = new();

    public async Task<IHLSHandler> GetOrAdd(SMChannelDto smChannel, string baseUrl)
    {
        if (hlsHandlers.ContainsKey(smChannel.Id))
        {
            return hlsHandlers[smChannel.Id];
        }

        IChannelStatus? channelStatus = await channelService.SetupChannel(smChannel);

        logger.LogInformation("Adding HLSHandler for {name}", smChannel.Name);
        HLSHandler hlsHandler = new(HLSHandlerlogger, FFMPEGRunnerlogger, channelService, smChannel, intSettings, inthlssettings);
        hlsHandler.Start(channelStatus.SMStream.Url);

        hlsHandler.ProcessExited += async (sender, args) =>
        {
            channelStatus = channelService.GetChannelStatus(smChannel.Id);
            //bool didSwitch = await channelService.SetNextChildVideoStream(channelStatus);
            if (!channelStatus.Shutdown)
            {
                hlsHandler.Start(channelStatus.SMStream.Url);
                logger.LogInformation("Switched to next stream for {Name}", smChannel.Name);
                return;
            }
            hlsHandler.Stop();
            logger.LogInformation("HLSHandler Process Exited for {Name} with exit code {ExitCode}", smChannel.Name, args.ExitCode);
            return;
        };
        hlsHandlers.TryAdd(smChannel.Id, hlsHandler);
        return hlsHandler;
    }

    private async Task Runner(IChannelStatus channelStatus)
    {
        while (await channelService.SwitchChannelToNextStream(channelStatus))
        {

        }
    }


    public IHLSHandler? Get(int smChannelId)
    {
        return hlsHandlers.GetValueOrDefault(smChannelId);
    }

    public void Stop(int smChannelId)
    {
        IHLSHandler? handler = Get(smChannelId);
        streamTracker.RemoveStream(smChannelId);
        if (handler is not null)
        {
            logger.LogInformation("Stopping HLSHandler for {name}", handler.SMChannel.Name);
            handler.Stop();
            IHLSHandler? hand = Get(smChannelId);
            if (hand is not null)
            {
                hlsHandlers.TryRemove(smChannelId, out _);
            }
        }
        HLSCancellationTokenSource.Cancel();
    }

    public void Dispose()
    {
        foreach (HLSHandler handler in hlsHandlers.Values)
        {
            handler.Stop();
            handler.Dispose();
        }
        hlsHandlers.Clear();
    }
}
