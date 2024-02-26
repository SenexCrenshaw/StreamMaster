using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Streams;

public class MP4HandlerBase(ILogger logger,
                            ILogger<FFMPEGRunner> FFMPEGRunnerlogger,
                            VideoStreamDto videoStream,
                            IOptionsMonitor<Setting> intsettings,
                            IOptionsMonitor<HLSSettings> inthlssettings)
{

    internal readonly CancellationTokenSource HLSCancellationTokenSource = new();
    internal readonly FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerlogger, intsettings, inthlssettings);
    internal bool Started;

    public string Id => videoStream.Id;
    public string Name => videoStream.User_Tvg_name;
    public string Url => videoStream.User_Url;

    public void Dispose()
    {
        ProcessHelper.KillProcessById(ffmpegRunner.ProcessId);
        HLSCancellationTokenSource.Cancel();
    }
}
