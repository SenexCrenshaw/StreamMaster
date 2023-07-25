using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;

using StreamMasterInfrastructure.Common;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StreamMasterInfrastructure.VideoStreamManager;

public static class StreamingProxies
{
    private static readonly HttpClient client = CreateHttpClient();

    public static async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetFFMpegStream(string streamUrl, ILogger logger)
    {
        Setting setting = FileUtil.GetSetting();

        var ffmpegExec = Path.Combine(Constants.ConfigFolder, setting.FFMPegExecutable);

        if (!File.Exists(ffmpegExec) && !File.Exists(ffmpegExec + ".exe"))
        {
            if (!IsFFmpegAvailable())
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"FFmpeg executable file not found: {setting.FFMPegExecutable}" };
                return (null, -1, error);
            }
            ffmpegExec = "ffmpeg";
        }

        try
        {
            using Process process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = $"-hide_banner -loglevel error -i \"{streamUrl}\" -c copy -f mpegts pipe:1 -user_agent \"{setting.ClientUserAgent}\"";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            _ = process.Start();
            logger.LogInformation("FFMpeg process started for stream: {StreamUrl}", setting.CleanURLs ? "url removed" : streamUrl);

            return (await Task.FromResult(process.StandardOutput.BaseStream).ConfigureAwait(false), process.Id, null);
        }
        catch (IOException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.IoError, Message = ex.Message };
            logger.LogError(error.Message);
            return (null, -1, error);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(error.Message);
            return (null, -1, error);
        }
    }

    public static async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(string sourceUrl, ILogger logger, CancellationToken cancellation)
    {
        Setting setting = FileUtil.GetSetting();
        try
        {
            HttpResponseMessage? response = await client.GetWithRedirectAsync(sourceUrl, cancellationToken: cancellation).ConfigureAwait(false);

            if (response == null || !response.IsSuccessStatusCode)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = "Could not retrieve stream url" };
                logger.LogError(error.Message);
                return (null, -1, error);
            }

            string contentType = response.Content.Headers.ContentType.MediaType;

            if (contentType is not null && contentType.Equals("application/vnd.apple.mpegurl", StringComparison.OrdinalIgnoreCase) ||
                        contentType.Equals("audio/mpegurl", StringComparison.OrdinalIgnoreCase) ||
                       contentType.Equals("application/x-mpegURL", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl}", setting.CleanURLs ? "url removed" : sourceUrl);
                return await GetFFMpegStream(sourceUrl, logger).ConfigureAwait(false);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(cancellation);
            logger.LogInformation("Successfully retrieved stream for: {StreamUrl}", setting.CleanURLs ? "url removed" : sourceUrl);
            return (stream, -1, null);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = ex.Message };
            logger.LogError(error.Message);
            return (null, -1, error);
        }
    }

    private static HttpClient CreateHttpClient()
    {
        var setting = FileUtil.GetSetting();
        var client = new HttpClient(new HttpClientHandler()
        {
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.UserAgent.ParseAdd(setting.StreamingClientUserAgent);
        return client;
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
        process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}
