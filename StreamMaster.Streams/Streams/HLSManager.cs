using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public class HLSManager(ILogger<HLSManager> logger, ILogger<HLSHandler> HLSHandlerlogger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings) : IHLSManager
{
    private readonly ConcurrentDictionary<string, IHLSHandler> hlsHandlers = new();

    private readonly CancellationTokenSource HLSCancellationTokenSource = new();

    public async Task<IHLSHandler> GetOrAdd(VideoStreamDto videoStream)
    {
        if (hlsHandlers.ContainsKey(videoStream.User_Url))
        {
            return hlsHandlers[videoStream.User_Url];
        }


        logger.LogInformation("Adding HLSHandler for {name}", videoStream.User_Tvg_name);
        HLSHandler hlsHandler = new(HLSHandlerlogger, FFMPEGRunnerlogger, videoStream, intsettings, inthlssettings);
        hlsHandler.Start();
        hlsHandler.ProcessExited += (sender, args) =>
        {
            logger.LogInformation("HLSHandler Process Exited for {Name} with exit code {ExitCode}", videoStream.User_Tvg_name, args.ExitCode);
            Stop(videoStream.Id);
        };
        hlsHandlers.TryAdd(videoStream.User_Url, hlsHandler);
        return hlsHandler;


    }


    public IHLSHandler? Get(string VideoStreamId)
    {
        return hlsHandlers.Values.FirstOrDefault(a => a.Id == VideoStreamId);
    }

    public void Stop(string VideoStreamId)
    {
        IHLSHandler? handler = Get(VideoStreamId);
        if (handler is not null)
        {
            logger.LogInformation("Stopping HLSHandler for {name}", handler.Name);
            handler.Stop();
            IHLSHandler? hand = Get(VideoStreamId);
            if (hand is not null)
            {
                hlsHandlers.TryRemove(hand.Url, out _);
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
