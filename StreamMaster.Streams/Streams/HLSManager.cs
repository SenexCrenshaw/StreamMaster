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
            return handler;
        });
    }

    public IHLSHandler? Get(string VideoStreamId)
    {
        return hlsHandlers.Values.FirstOrDefault(a => a.Id == VideoStreamId);
    }

    //public void Start(string url)
    //{
    //    if (hlsHandlers.TryGetValue(url, out HLSHandler? handler))
    //    {
    //        logger.LogInformation("Starting HLSHandler for {url}", url);
    //        handler.Start();
    //    }
    //}

    public void Stop(string VideoStreamId)
    {
        IHLSHandler? handler = Get(VideoStreamId);
        if (handler is not null)
        {
            logger.LogInformation("Stopping HLSHandler for {name}", handler.Name);
            handler.Stop();
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
