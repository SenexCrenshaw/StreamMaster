using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;


using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class HLSHandler(ILogger<HLSHandler> logger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, VideoStreamDto videoStream, IMemoryCache memoryCache) : IHLSHandler, IDisposable
{
    private readonly CancellationTokenSource HLSCancellationTokenSource = new();
    private readonly FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerlogger, memoryCache);
    private bool Started;
    private int processId;

    public string Id => videoStream.Id;
    public string Name => videoStream.User_Tvg_name;

    public void Start()
    {
        if (Started)
        {
            return;
        }

        logger.LogInformation("Starting HLSHandler for {Name}", videoStream.User_Tvg_name);
        (processId, ProxyStreamError? error) = ffmpegRunner.CreateFFMpegHLS(videoStream);
        if (processId == -1)
        {
            logger.LogError("Failed to start HLSHandler for {Name} {error}", videoStream.User_Tvg_name, error);
            return;
        }
        Started = true;
    }


    public void Stop()
    {
        Started = false;
        logger.LogInformation("Stopping HLSHandler for {Name}", videoStream.User_Tvg_name);
        HLSCancellationTokenSource.Cancel();
        KillProcess();

        string directory = Path.Combine(BuildInfo.HLSOutputFolder, Id);
        if (Directory.Exists(directory))
        {
            DirectoryHelper.DeleteAllFiles(directory, logger);
        }
    }

    private bool KillProcess()
    {
        if (processId == -1)
        {
            return true;
        }
        try
        {
            Process process = Process.GetProcessById(processId);
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
