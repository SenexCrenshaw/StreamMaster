using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Streams;

public class MP4HandlerBase(ILogger logger,
                            ILogger<FFMPEGRunner> FFMPEGRunnerlogger,
                            SMStream smStream,
                            IOptionsMonitor<Setting> intsettings,
                            IOptionsMonitor<HLSSettings> inthlssettings)
{

    internal readonly CancellationTokenSource HLSCancellationTokenSource = new();
    internal readonly FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerlogger, intsettings, inthlssettings);
    internal bool Started;

    public string Id => smStream.Id;
    public string Name => smStream.Name;
    public string Url => smStream.Url;

    public void Dispose()
    {
        ProcessHelper.KillProcessById(ffmpegRunner.ProcessId);
        HLSCancellationTokenSource.Cancel();
    }
}
