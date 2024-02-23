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
    private Process process;
    public (int processId, ProxyStreamError? error) CreateFFMpegHLS(VideoStreamDto videoStream)
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
            outputdir += Path.DirectorySeparatorChar;

            if (!Directory.Exists(outputdir))
            {
                Directory.CreateDirectory(outputdir);
            }
            string formattedArgs = $"-i \"{videoStream.User_Url}\"";

            formattedArgs += $" -c:a copy " +
                     "-c:v copy " +
                     "-sn " +
                     "-flags -global_header " +
                     "-avoid_negative_ts disabled " +
                     "-map_metadata -1 " +
                     "-start_at_zero " +
                     "-copyts " +
                     "-flags -global_header " +
                     "-vsync cfr " +
                     "-y " +
                     "-nostats " +
                     "-hide_banner " +
                     "-f hls " +
                     "-hls_segment_type mpegts " +
                     "-hls_init_time 1 " +
                     "-hls_allow_cache 1 " +
                     "-hls_flags +omit_endlist " +
                     "-hls_flags +round_durations " +
                     "-hls_flags +discont_start " +
                     "-hls_flags +delete_segments " +
                     $"-user_agent \"{settings.StreamingClientUserAgent}\"";

            formattedArgs += $" -hls_time {settings.HLSSegmentDurantionInSeconds}" +
                              $" -hls_list_size {settings.HLSSegmentCount}";

            formattedArgs += $" -hls_base_url \"{videoStream.Id}/\"" +
                              $" -hls_segment_filename \"{outputdir}%d.ts\"" +
                              $" \"{outputdir}index.m3u8\"";

            process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = formattedArgs;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.Exited += OnProcessExited;

            bool processStarted = process.Start();

            if (!processStarted)
            {
                // Log and return an error if the process couldn't be started
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);

                return (-1, error);
            }

            //StreamReader myStreamReader = process.StandardError;
            //string standardError = myStreamReader.ReadToEnd();
            //if (!string.IsNullOrEmpty(standardError))
            //{
            //    ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = standardError };
            //    logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);

            //    return (-1, error);
            //}

            // Return the standard output stream of the process

            logger.LogInformation("Opened ffmpeg stream for {streamName} with args \"{formattedArgs}\"", videoStream.User_Tvg_name, formattedArgs);
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

    protected virtual void OnProcessExited(object? sender, EventArgs e)
    {
        logger.LogInformation("FFMpeg process exited with code {ExitCode}", process.ExitCode);
        ProcessExited?.Invoke(this, new ProcessExitEventArgs { ExitCode = process.ExitCode });
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

            bool processStarted = process.Start();
            stopwatch.Stop();
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
