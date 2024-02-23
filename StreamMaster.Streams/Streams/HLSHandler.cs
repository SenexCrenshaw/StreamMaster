using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Models;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class HLSHandler(ILogger<HLSHandler> logger, ILogger<FFMPEGRunner> FFMPEGRunnerlogger, VideoStreamDto videoStream, IMemoryCache memoryCache) : IHLSHandler, IDisposable
{
    public event EventHandler<ProcessExitEventArgs> ProcessExited;
    private readonly CancellationTokenSource HLSCancellationTokenSource = new();
    private readonly FFMPEGRunner ffmpegRunner = new(FFMPEGRunnerlogger, memoryCache);
    private bool Started;


    public string Id => videoStream.Id;
    public string Name => videoStream.User_Tvg_name;
    public string Url => videoStream.User_Url;
    public async Task Start()
    {
        if (Started)
        {
            return;
        }

        logger.LogInformation("Starting HLSHandler for {Name}", videoStream.User_Tvg_name);

        Task backgroundTask = ffmpegRunner.StartStreamingInBackgroundAsync(videoStream, HLSCancellationTokenSource.Token);

        //processId = ffmpegRunner.ProcessId;
        ////(processId, ProxyStreamError? error) = await ffmpegRunner.CreateFFMpegHLS(videoStream, HLSCancellationTokenSource.Token);
        //if (processId == -1)
        //{
        //    logger.LogError("Failed to start HLSHandler for {Name} {error}", videoStream.User_Tvg_name);
        //    return;
        //}

        ffmpegRunner.ProcessExited += (sender, args) =>
        {
            logger.LogInformation("FFMPEG Process Exited for {Name} with exit code {ExitCode}", videoStream.User_Tvg_name, args.ExitCode);
            Stop();
            ProcessExited?.Invoke(this, args);
        };
        Started = true;
    }


    public void Stop()
    {
        Started = false;
        logger.LogInformation("Stopping HLSHandler for {Name}", videoStream.User_Tvg_name);
        HLSCancellationTokenSource.Cancel();
        KillProcess();

        string directory = Path.Combine(BuildInfo.HLSOutputFolder, Id);
        DirectoryHelper.DeleteDirectory(directory, logger);
    }

    private bool KillProcess()
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
