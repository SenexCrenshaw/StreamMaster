using StreamMaster.Domain.Configuration;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StreamMaster.Streams.Streams;
public class FFMPEGRunner(ILogger<FFMPEGRunner> logger, IChannelService channelService, IOptionsMonitor<Setting> intSettings, IOptionsMonitor<HLSSettings> intHLSSettings)
    : IFFMPEGRunner
{

    public event EventHandler<ProcessExitEventArgs> ProcessExited;
    private string? GetFFPMpegExec()
    {
        Setting settings = intSettings.CurrentValue;
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
    public Task HLSStartStreamingInBackgroundAsync(SMChannel smChannel, string url, CancellationToken cancellationToken)
    {
        IChannelStatus? status = channelService.GetChannelStatus(smChannel.Id);

        // Start the streaming task without awaiting it here, letting it run in the background
        Task<(int processId, ProxyStreamError? error)> streamingTask = Task.Run(() => CreateFFMpegHLS(smChannel, url), cancellationToken);

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
        }, TaskScheduler.Default);

        return streamingTask; // Return the task for optional further management
    }

    private Process process;
    private async Task<(int processId, ProxyStreamError? error)> CreateFFMpegHLS(SMChannel smChannel, string url)
    {
        Setting settings = intSettings.CurrentValue;
        HLSSettings hlssettings = intHLSSettings.CurrentValue;
        try
        {
            string? ffmpegExec = GetFFPMpegExec();
            if (ffmpegExec == null)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);

                return (-1, error);
            }

            string outputdir = Path.Combine(BuildInfo.HLSOutputFolder, smChannel.Id.ToString());

            if (!outputdir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                outputdir += Path.DirectorySeparatorChar.ToString();
            }

            if (!Directory.Exists(outputdir))
            {
                Directory.CreateDirectory(outputdir);
            }

            string formattedArgs = hlssettings.HLSFFMPEGOptions.Replace("{streamUrl}", $"\"{url}\"").Trim();
            formattedArgs = formattedArgs += " ";

            formattedArgs +=
       $"-reconnect_delay_max {hlssettings.HLSReconnectDurationInSeconds} " +
       $"-user_agent \"{settings.StreamingClientUserAgent}\" ";


            formattedArgs += $"-hls_time {hlssettings.HLSSegmentDurationInSeconds} " +
                              $"-hls_list_size {hlssettings.HLSSegmentCount} " +
                              $"-hls_delete_threshold {hlssettings.HLSSegmentCount} ";

            formattedArgs += $"-hls_base_url \"{smChannel.Id}/\" " +
                             $"-hls_segment_filename \"{outputdir}%d.ts\" " +
                             $"\"{outputdir}index.m3u8\"";

            process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = formattedArgs;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            //process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;
            //process.ErrorDataReceived += (sender, args) => logger.LogDebug(args.Data);
            bool processStarted = process.Start();

            process.BeginErrorReadLine();

            if (!processStarted)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);

                return (-1, error);
            }

            logger.LogInformation("Opened ffmpeg stream for {streamName} with args \"{formattedArgs}\"", smChannel.Name, formattedArgs);
            await process.WaitForExitAsync().ConfigureAwait(false);
            return (process.Id, null);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "CreateFFMpegHLS Error: {ErrorMessage}", error.Message);
            return (-1, error);
        }
    }

    private void Process_Exited(object? sender, EventArgs e)
    {
        ProcessExited?.Invoke(this, new ProcessExitEventArgs { ExitCode = process.ExitCode });
    }


    private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        string? msg = e.Data;
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }
        logger.LogDebug(msg);
    }
    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -i {streamUrl} -c copy -bsf:v h264_mp4toannexb -f mpegts pipe:1";

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStream(string streamUrl, string streamName)
    {
        Setting settings = intSettings.CurrentValue;
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

            string options = string.IsNullOrEmpty(FFMpegOptions) ? BuildInfo.FFMPEGDefaultOptions : FFMpegOptions;

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

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStreamByArgs(string args, string streamName)
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

            //string options = string.IsNullOrEmpty(FFMpegOptions) ? BuildInfo.FFMPEGDefaultOptions : FFMpegOptions;

            //string formattedArgs = options.Replace("{streamUrl}", $"\"{streamUrl}\"");
            //formattedArgs += $" -user_agent \"{settings.StreamingClientUserAgent}\"";

            process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = args;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => ProcessExited?.Invoke(this, new ProcessExitEventArgs { ExitCode = process.ExitCode });
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

            logger.LogInformation("Opened ffmpeg stream for {streamName} with args \"{args}\" in {ElapsedMilliseconds} ms", streamName, args, stopwatch.ElapsedMilliseconds);

            return (await Task.FromResult(process.StandardOutput.BaseStream).ConfigureAwait(false), process.Id, null);
        }
        catch (Exception ex)
        {
            // Log and return any exceptions that occur
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "CreateFFMpegStream by args {args} Error: {ErrorMessage}", args, error.Message);
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
