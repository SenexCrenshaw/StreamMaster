using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Models;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StreamMaster.Streams.Streams;
public class FFMPEGRunner(ILogger<FFMPEGRunner> logger, IMemoryCache memoryCache)
{
    public event EventHandler<ProcessExitEventArgs> ProcessExited;
    private string? GetFFPMpegExec()
    {
        Setting settings = memoryCache.GetSetting();

        string ffmpegExec = Path.Combine(BuildInfo.AppDataFolder, settings.FFMPegExecutable);

        if (!File.Exists(ffmpegExec) && !File.Exists(ffmpegExec + ".exe"))
        {
            if (!IsFFmpegAvailable())
            {
                logger.LogError($"GetFFPMpegExec FFmpeg executable file not found: {settings.FFMPegExecutable}");
                return null;
            }
            ffmpegExec = "ffmpeg";
        }


        return ffmpegExec;
    }

    public int ProcessId => process.Id;

    // Start the streaming process in the background
    public Task HLSStartStreamingInBackgroundAsync(VideoStreamDto videoStream, CancellationToken cancellationToken)
    {
        // Start the streaming task without awaiting it here, letting it run in the background
        Task<(int processId, ProxyStreamError? error)> streamingTask = Task.Run(() => CreateFFMpegHLS(videoStream, cancellationToken), cancellationToken);

        // Optionally handle completion, including logging or re-throwing errors
        streamingTask.ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                // Log the exception or handle it as needed
                logger.LogError($"Streaming task failed: {task.Exception?.GetBaseException().Message}");
            }
            else if (task.IsCompletedSuccessfully)
            {
                logger.LogInformation("Streaming task completed successfully.");
            }
            ProcessExited?.Invoke(this, new ProcessExitEventArgs { ExitCode = process.ExitCode });
            // No need to handle task.IsCanceled separately as OperationCanceledException will be caught in StartStreamingAsync
        }, TaskScheduler.Default);

        return streamingTask; // Return the task for optional further management
    }

    private Process process;
    private async Task<(int processId, ProxyStreamError? error)> CreateFFMpegHLS(VideoStreamDto videoStream, CancellationToken cancellationToken)
    {
        try
        {
            string? ffmpegExec = GetFFPMpegExec();
            if (ffmpegExec == null)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);

                return (-1, error);
            }

            Setting settings = memoryCache.GetSetting();

            string outputdir = Path.Combine(BuildInfo.HLSOutputFolder, videoStream.Id);

            if (BuildInfo.IsWindows)
            {
                outputdir = "c:" + outputdir;
            }

            if (!outputdir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                outputdir += Path.DirectorySeparatorChar.ToString();
            }

            if (!Directory.Exists(outputdir))
            {
                Directory.CreateDirectory(outputdir);
            }

            string formattedArgs = settings.HLS.HLSFFMPEGOptions.Replace("{streamUrl}", $"\"{videoStream.User_Url}\"").Trim();
            formattedArgs = formattedArgs += " ";

            formattedArgs +=
       $"-reconnect_delay_max {settings.HLS.HLSReconnectDurationInSeconds} " +
       $"-user_agent \"{settings.StreamingClientUserAgent}\" ";


            formattedArgs += $"-hls_time {settings.HLS.HLSSegmentDurationInSeconds} " +
                              $"-hls_list_size {settings.HLS.HLSSegmentCount} " +
                              $"-hls_delete_threshold {settings.HLS.HLSSegmentCount} ";

            formattedArgs += $"-hls_base_url \"{videoStream.Id}/\" " +
                             $"-hls_segment_filename \"{outputdir}%d.ts\" " +
                             $"\"{outputdir}index.m3u8\"";

            process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = formattedArgs;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            //process.EnableRaisingEvents = true;
            //process.Exited += OnProcessExited;
            process.ErrorDataReceived += (sender, args) => logger.LogDebug(args.Data);
            bool processStarted = process.Start();

            process.BeginErrorReadLine();

            if (!processStarted)
            {
                // Log and return an error if the process couldn't be started
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);

                return (-1, error);
            }

            logger.LogInformation("Opened ffmpeg stream for {streamName} with args \"{formattedArgs}\"", videoStream.User_Tvg_name, formattedArgs);
            await process.WaitForExitAsync().ConfigureAwait(false);
            return (process.Id, null);
        }
        catch (Exception ex)
        {
            // Log and return any exceptions that occur
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "CreateFFMpegHLS Error: {ErrorMessage}", error.Message);
            return (-1, error);
        }
    }

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStream(string streamUrl, string streamName)
    {
        try
        {
            string? ffmpegExec = GetFFPMpegExec();
            if (ffmpegExec == null)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);

                return (null, -1, error);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Setting settings = memoryCache.GetSetting();

            string options = string.IsNullOrEmpty(settings.FFMpegOptions) ? BuildInfo.FFMPEGDefaultOptions : settings.FFMpegOptions;

            string formattedArgs = options.Replace("{streamUrl}", $"\"{streamUrl}\"");
            formattedArgs += $" -user_agent \"{settings.StreamingClientUserAgent}\"";

            process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = formattedArgs;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) =>
            {
                ProcessExited?.Invoke(this, new ProcessExitEventArgs { ExitCode = process.ExitCode });
            };
            process.ErrorDataReceived += (sender, args) => logger.LogDebug(args.Data);
            bool processStarted = process.Start();
            process.BeginErrorReadLine();

            if (!processStarted)
            {
                // Log and return an error if the process couldn't be started
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegStream Error: {ErrorMessage}", error.Message);

                return (null, -1, error);
            }

            // Return the standard output stream of the process

            logger.LogInformation("Opened ffmpeg stream for {streamName} with args \"{formattedArgs}\" in {ElapsedMilliseconds} ms", streamName, formattedArgs, stopwatch.ElapsedMilliseconds);

            return (await Task.FromResult(process.StandardOutput.BaseStream).ConfigureAwait(false), process.Id, null);
        }
        catch (Exception ex)
        {
            // Log and return any exceptions that occur
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "CreateFFMpegStream Error: {ErrorMessage}", error.Message);
            return (null, -1, error);
        }
    }

    private static bool IsFFmpegAvailable()
    {
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        ProcessStartInfo startInfo = new(command, "ffmpeg")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        Process process = new()
        {
            StartInfo = startInfo
        };
        _ = process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}
