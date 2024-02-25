using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class MP4HandlerBase(ILogger logger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, VideoStreamDto videoStream, IMemoryCache memoryCache)
{

    internal readonly CancellationTokenSource HLSCancellationTokenSource = new();
    internal readonly FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerlogger, memoryCache);
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
