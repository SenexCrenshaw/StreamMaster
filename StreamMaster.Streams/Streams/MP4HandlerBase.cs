using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Streams;

public class MP4HandlerBase(ILogger<FFMPEGRunner> FFMPEGRunnerLogger,
     IChannelService channelService,
                            SMChannel smChannel,
                            IOptionsMonitor<Setting> intSettings,
                            IOptionsMonitor<HLSSettings> intHLSSettings)
{

    internal readonly CancellationTokenSource HLSCancellationTokenSource = new();
    internal readonly FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerLogger, channelService, intSettings, intHLSSettings);
    internal bool Started;

    public SMChannel SMChannel => smChannel;
    public void Dispose()
    {
        ProcessHelper.KillProcessById(ffmpegRunner.ProcessId);
        HLSCancellationTokenSource.Cancel();
    }
}
