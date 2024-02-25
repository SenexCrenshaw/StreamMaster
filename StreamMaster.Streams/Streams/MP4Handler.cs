using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Streams;
public class MP4Handler(ILogger<MP4Handler> logger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, VideoStreamDto parentvideoStream, IMemoryCache memoryCache)
    : MP4HandlerBase(logger, FFMPEGRunnerlogger, parentvideoStream, memoryCache), IHLSHandler, IDisposable
{
    public event EventHandler<ProcessExitEventArgs> ProcessExited;

    public Stream? Stream { get; private set; }
    public async Task Start()
    {
        if (Started)
        {
            return;
        }

        logger.LogInformation("Starting MP4Handler for {Name}", Name);

        (Stream, int processId, ProxyStreamError? error) = await ffmpegRunner.CreateFFMpegStream(Url, Name);

        ffmpegRunner.ProcessExited += (sender, args) =>
        {
            logger.LogInformation("FFMPEG Process Exited for {Name} with exit code {ExitCode}", Name, args.ExitCode);
            Stop();
            ProcessExited?.Invoke(this, args);
        };
        Started = true;
    }
    public void Stop()
    {
        Started = false;
        logger.LogInformation("Stopping MP4Handler for {Name}", Name);
        HLSCancellationTokenSource.Cancel();
        KillProcess();

        string directory = Path.Combine(BuildInfo.HLSOutputFolder, Id);
        DirectoryHelper.DeleteDirectory(directory, logger);
    }

    public void Dispose()
    {
        HLSCancellationTokenSource.Cancel();
    }
}
