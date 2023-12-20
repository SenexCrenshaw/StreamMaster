using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Services;

using StreamMasterInfrastructure.Common;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StreamMasterInfrastructure.VideoStreamManager.Factories;

public sealed class ProxyFactory(ILogger<ProxyFactory> logger, IHttpClientFactory httpClientFactory, ISettingsService settingsService) : IProxyFactory
{
    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(string streamUrl, string streamName, CancellationToken cancellationToken)
    {
        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

        Stream? stream;
        ProxyStreamError? error;
        int processId;

        if (setting.StreamingProxyType == StreamingProxyTypes.FFMpeg)
        {
            (stream, processId, error) = await GetFFMpegStream(streamUrl);
            LogErrorIfAny(stream, error, streamUrl, streamName);
        }
        else
        {
            (stream, processId, error) = await GetProxyStream(streamUrl, streamName, cancellationToken);
            LogErrorIfAny(stream, error, streamUrl, streamName);
        }

        return (stream, processId, error);
    }

    private void LogErrorIfAny(Stream? stream, ProxyStreamError? error, string streamUrl, string streamName)
    {
        if (stream == null || error != null)
        {
            logger.LogError("Error getting proxy stream for {StreamUrl} {streamName}: {ErrorMessage}", streamUrl, streamName, error?.Message);
        }
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetFFMpegStream(string streamUrl)
    {
        Setting settings = await settingsService.GetSettingsAsync().ConfigureAwait(false);

        string ffmpegExec = Path.Combine(BuildInfo.AppDataFolder, settings.FFMPegExecutable);

        if (!File.Exists(ffmpegExec) && !File.Exists(ffmpegExec + ".exe"))
        {
            if (!IsFFmpegAvailable())
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"FFmpeg executable file not found: {settings.FFMPegExecutable}" };
                return (null, -1, error);
            }
            ffmpegExec = "ffmpeg";
        }

        try
        {
            return await CreateFFMpegStream(ffmpegExec, streamUrl).ConfigureAwait(false);
        }
        catch (IOException ex)
        {
            return HandleFFMpegStreamException(ProxyStreamErrorCode.IoError, ex);
        }
        catch (Exception ex)
        {
            return HandleFFMpegStreamException(ProxyStreamErrorCode.UnknownError, ex);
        }
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStream(string ffmpegExec, string streamUrl)
    {
        try
        {
            Setting settings = await settingsService.GetSettingsAsync().ConfigureAwait(false);

            string options = string.IsNullOrEmpty(settings.FFMpegOptions) ? BuildInfo.FFMPEGDefaultOptions : settings.FFMpegOptions;

            string formattedArgs = options.Replace("{streamUrl}", $"\"{streamUrl}\"");
            formattedArgs += $" -user_agent \"{settings.StreamingClientUserAgent}\"";

            using Process process = new();
            process.StartInfo.FileName = ffmpegExec;
            process.StartInfo.Arguments = formattedArgs;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            bool processStarted = process.Start();
            if (!processStarted)
            {
                // Log and return an error if the process couldn't be started
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                logger.LogError("CreateFFMpegStream Error: {ErrorMessage}", error.Message);
                return (null, -1, error);
            }

            // Return the standard output stream of the process
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

    private (Stream? stream, int processId, ProxyStreamError? error) HandleFFMpegStreamException<T>(ProxyStreamErrorCode errorCode, T exception) where T : Exception
    {
        ProxyStreamError error = new() { ErrorCode = errorCode, Message = exception.Message };
        logger.LogError(exception, "GetFFMpegStream Error: {message}", error.Message);
        return (null, -1, error);
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(string sourceUrl, string streamName, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

        try
        {
            Setting settings = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
            HttpClient client = CreateHttpClient(settings.StreamingClientUserAgent);
            HttpResponseMessage? response = await client.GetWithRedirectAsync(sourceUrl, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (response?.IsSuccessStatusCode != true)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = $"Could not retrieve stream for {streamName} {response?.StatusCode}", };
                logger.LogError("GetProxyStream Error: {message}", error.Message);
                return (null, -1, error);
            }

            string? contentType = response.Content.Headers?.ContentType?.MediaType;

            if ((!string.IsNullOrEmpty(contentType) &&
                    contentType.Equals("application/vnd.apple.mpegurl", StringComparison.OrdinalIgnoreCase)) ||
                    contentType.Equals("audio/mpegurl", StringComparison.OrdinalIgnoreCase) ||
                    contentType.Equals("application/x-mpegURL", StringComparison.OrdinalIgnoreCase)
                )
            {
                logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl} {streamName}", sourceUrl, streamName);
                return await GetFFMpegStream(sourceUrl).ConfigureAwait(false);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            stopwatch.Stop(); // Stop the stopwatch when the stream is retrieved
            logger.LogInformation("Successfully retrieved stream for: {StreamUrl} {streamName} in {ElapsedMilliseconds} ms", sourceUrl, streamName, stopwatch.ElapsedMilliseconds);

            return (stream, -1, null);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = ex.Message };
            string message = $"GetProxyStream Error for {streamName}";
            logger.LogError(ex, message, error.Message);
            return (null, -1, error);
        }
    }

    private HttpClient CreateHttpClient(string streamingClientUserAgent)
    {
        HttpClient client = httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.UserAgent.ParseAdd(streamingClientUserAgent);
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
        _ = process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}
