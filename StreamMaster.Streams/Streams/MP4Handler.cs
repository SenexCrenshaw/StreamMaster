using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Streams;
public class MP4Handler(ILogger<MP4Handler> logger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, SMStreamDto parentvideoStream, IOptionsMonitor<HLSSettings> inthlssettings, IOptionsMonitor<Setting> intsettings)
    : MP4HandlerBase(logger, FFMPEGRunnerlogger, parentvideoStream, intsettings, inthlssettings), IHLSHandler, IDisposable
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
        ProcessHelper.KillProcessById(ffmpegRunner.ProcessId);

        string directory = Path.Combine(BuildInfo.HLSOutputFolder, Id);
        DirectoryHelper.DeleteDirectory(directory);
    }

    public void Dispose()
    {
        HLSCancellationTokenSource.Cancel();
    }
}
