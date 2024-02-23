using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;

namespace StreamMaster.Streams.Streams;

public class HLSManager(ILogger<HLSManager> logger, ILogger<HLSHandler> HLSHandlerlogger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, IMemoryCache memoryCache) : IHLSManager
{
    private readonly ConcurrentDictionary<string, HLSHandler> hlsHandlers = new();

    private readonly CancellationTokenSource HLSCancellationTokenSource = new();

    public IHLSHandler GetOrAdd(VideoStreamDto videoStream)
    {
        return hlsHandlers.GetOrAdd(videoStream.User_Url, _ =>
        {
            logger.LogInformation("Adding HLSHandler for {name}", videoStream.User_Tvg_name);
            HLSHandler handler = new(HLSHandlerlogger, FFMPEGRunnerlogger, videoStream, memoryCache);
            handler.Start();
            handler.ProcessExited += (sender, args) =>
            {
                logger.LogInformation("HLSHandler Process Exited for {Name} with exit code {ExitCode}", videoStream.User_Tvg_name, args.ExitCode);
                hlsHandlers.TryRemove(videoStream.User_Url, out HLSHandler? _);
            };
            return handler;
        });
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
            hlsHandlers.TryRemove(handler.Url, out _);
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
