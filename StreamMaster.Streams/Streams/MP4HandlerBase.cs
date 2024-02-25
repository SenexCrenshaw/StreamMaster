using StreamMaster.Domain.Configuration;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class MP4HandlerBase(ILogger logger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, VideoStreamDto videoStream, IOptionsMonitor<Setting> intsettings)
{

    internal readonly CancellationTokenSource HLSCancellationTokenSource = new();
    internal readonly FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerlogger, intsettings);
    internal bool Started;

    public string Id => videoStream.Id;
    public string Name => videoStream.User_Tvg_name;
    public string Url => videoStream.User_Url;


    internal bool KillProcess()
    {
        if (ffmpegRunner.ProcessId < 1024)
        {
            return true;
        }
        try
        {
            Process process = Process.GetProcessById(ffmpegRunner.ProcessId);
            process.Kill();
            return true;
        }
        catch (ArgumentException)
        {

        }
        return false;


    }

    public void Dispose()
    {
        KillProcess();
        HLSCancellationTokenSource.Cancel();

    }
}
