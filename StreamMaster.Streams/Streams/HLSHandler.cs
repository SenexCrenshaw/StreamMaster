using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Streams;

public class HLSHandler(ILogger<HLSHandler> logger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, IChannelService channelService, SMChannel smChannel, IOptionsMonitor<Setting> intSettings, IOptionsMonitor<HLSSettings> intHLSSettings)
     : MP4HandlerBase(FFMPEGRunnerlogger, channelService, smChannel, intSettings, intHLSSettings), IHLSHandler, IDisposable
{
    public event EventHandler<ProcessExitEventArgs> ProcessExited;
    public Stream? Stream { get; }
    public void Start(string url)
    {
        if (Started)
        {
            return;
        }

        logger.LogInformation("Starting MP4Handler for {Name}", SMChannel.Name);

        Task backgroundTask = ffmpegRunner.HLSStartStreamingInBackgroundAsync(SMChannel, url, HLSCancellationTokenSource.Token);

        ffmpegRunner.ProcessExited += (sender, args) =>
        {
            logger.LogInformation("FFMPEG Process Exited for {Name} with exit code {ExitCode}", SMChannel.Name, args.ExitCode);
            //Stop();
            ProcessExited?.Invoke(this, args);
        };
        Started = true;
    }
    public void Stop()
    {
        Started = false;
        logger.LogInformation("Stopping MP4Handler for {Name}", SMChannel.Name);
        HLSCancellationTokenSource.Cancel();
        ProcessHelper.KillProcessById(ffmpegRunner.ProcessId);

        string directory = Path.Combine(BuildInfo.HLSOutputFolder, SMChannel.Id.ToString());
        DirectoryHelper.DeleteDirectory(directory);
    }
    public new void Dispose()
    {
        HLSCancellationTokenSource.Cancel();

    }
}
