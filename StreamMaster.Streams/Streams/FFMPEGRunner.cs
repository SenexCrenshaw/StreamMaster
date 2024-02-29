using StreamMaster.Domain.Configuration;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StreamMaster.Streams.Streams;
public class FFMPEGRunner(ILogger<FFMPEGRunner> logger, IOptionsMonitor<Setting> intsettings, IOptionsMonitor<HLSSettings> inthlssettings)
{
    private readonly HLSSettings hlssettings = inthlssettings.CurrentValue;
    private readonly Setting settings = intsettings.CurrentValue;
    // private static readonly Regex ProgressRegex = new(@"time=(\d\d:\d\d:\d\d.\d\d?)", RegexOptions.Compiled);
    // private Action<double>? _onPercentageProgress;
    // private Action<TimeSpan>? _onTimeProgress;
    // private Action<string>? _onOutput;
    // private Action<string>? _onError;
    // private TimeSpan? _totalTimespan;

    public event EventHandler<ProcessExitEventArgs> ProcessExited;
    private string? GetFFPMpegExec()
    {

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
            //ProcessExited?.Invoke(this, new ProcessExitEventArgs { ExitCode = process.ExitCode });
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

            string outputdir = Path.Combine(BuildInfo.HLSOutputFolder, videoStream.Id);

            if (!outputdir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                outputdir += Path.DirectorySeparatorChar.ToString();
            }

            if (!Directory.Exists(outputdir))
            {
                Directory.CreateDirectory(outputdir);
            }

            string formattedArgs = hlssettings.HLSFFMPEGOptions.Replace("{streamUrl}", $"\"{videoStream.User_Url}\"").Trim();
            formattedArgs = formattedArgs += " ";

            formattedArgs +=
       $"-reconnect_delay_max {hlssettings.HLSReconnectDurationInSeconds} " +
       $"-user_agent \"{settings.StreamingClientUserAgent}\" ";


            formattedArgs += $"-hls_time {hlssettings.HLSSegmentDurationInSeconds} " +
                              $"-hls_list_size {hlssettings.HLSSegmentCount} " +
                              $"-hls_delete_threshold {hlssettings.HLSSegmentCount} ";

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

            //process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;
            //process.ErrorDataReceived += (sender, args) => logger.LogDebug(args.Data);
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

    private void Process_Exited(object? sender, EventArgs e)
    {
        //_onPercentageProgress?.Invoke(100.0);
        //if (_totalTimespan.HasValue)
        //{
        //    _onTimeProgress?.Invoke(_totalTimespan.Value);
        //}
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

        //_onError?.Invoke(msg);

        //Match match = ProgressRegex.Match(msg);
        //if (!match.Success)
        //{
        //    return;
        //}

        //TimeSpan processed = TimeSpan.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        //_onTimeProgress?.Invoke(processed);

        //if (_onPercentageProgress == null || _totalTimespan == null)
        //{
        //    return;
        //}

        //double percentage = Math.Round(processed.TotalSeconds / _totalTimespan.Value.TotalSeconds * 100, 2);
        //_onPercentageProgress(percentage);
    }

    //private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    //{
    //    string? msg = e.Data;
    //    if (string.IsNullOrEmpty(msg))
    //    {
    //        return;
    //    }
    //    Debug.WriteLine(msg);
    //    //_onOutput?.Invoke(msg);
    //}

    /// <summary>
    /// Register action that will be invoked during the ffmpeg processing, when a progress time is output and parsed and progress percentage is calculated.
    /// Total time is needed to calculate the percentage that has been processed of the full file.
    /// </summary>
    /// <param name="onPercentageProgress">Action to invoke when progress percentage is updated</param>
    /// <param name="totalTimeSpan">The total timespan of the mediafile being processed</param>
    //public FFMPEGRunner NotifyOnProgress(Action<double> onPercentageProgress, TimeSpan totalTimeSpan)
    //{
    //    _totalTimespan = totalTimeSpan;
    //    _onPercentageProgress = onPercentageProgress;
    //    return this;
    //}
    /// <summary>
    /// Register action that will be invoked during the ffmpeg processing, when a progress time is output and parsed
    /// </summary>
    /// <param name="onTimeProgress">Action that will be invoked with the parsed timestamp as argument</param>
    //public FFMPEGRunner NotifyOnProgress(Action<TimeSpan> onTimeProgress)
    //{
    //    _onTimeProgress = onTimeProgress;
    //    return this;
    //}

    /// <summary>
    /// Register action that will be invoked during the ffmpeg processing, when a line is output
    /// </summary>
    /// <param name="onOutput"></param>
    //public FFMPEGRunner NotifyOnOutput(Action<string> onOutput)
    //{
    //    _onOutput = onOutput;
    //    return this;
    //}
    //public FFMPEGRunner NotifyOnError(Action<string> onError)
    //{
    //    _onError = onError;
    //    return this;
    //}


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
