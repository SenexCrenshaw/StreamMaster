using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

using System.Diagnostics;
using System.Runtime.InteropServices;
namespace StreamMaster.Streams.Factories;

public sealed class ProxyFactory(ILogger<ProxyFactory> logger, IHttpClientFactory httpClientFactory, IOptionsMonitor<Setting> intSettings)
    : IProxyFactory
{

    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -bsf:v h264_mp4toannexb -f mpegts pipe:1";

    private string GetStreamingProxyType(string videoStreamStreamingProxyType)
    {
        Setting settings = intSettings.CurrentValue;
        return videoStreamStreamingProxyType == "SystemDefault"
            ? settings.StreamingProxyType
            : videoStreamStreamingProxyType;
    }

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(SMStreamDto smStream, string clientUserAgent, CancellationToken cancellationToken)
    {
        Stream? stream;
        ProxyStreamError? error;
        int processId;

        //string proxyType = GetStreamingProxyType(smStream);

        //if (proxyType == "None")
        //{
        //    logger.LogInformation("No proxy stream needed for {StreamUrl} {streamName}", smStream.Url, smStream.Name);
        //    return (null, -1, null);
        //}
        //if (proxyType == "StreamMaster")
        //{
        //    (stream, processId, error) = await GetProxyStream(streamUrl, streamName);
        //    LogErrorIfAny(stream, error, streamUrl, streamName);
        //}
        //else
        //{
        //    (stream, processId, error) = GetCommandStream(streamUrl);
        //    LogErrorIfAny(stream, error, streamUrl, streamName);
        //}
        (stream, processId, error) = await GetProxyStream(smStream, clientUserAgent, cancellationToken);
        LogErrorIfAny(stream, error, smStream.Url, smStream.Name);
        return (stream, processId, error);
    }

    private void LogErrorIfAny(Stream? stream, ProxyStreamError? error, string streamUrl, string streamName)
    {
        if (stream == null || error != null)
        {
            logger.LogError("Error getting proxy stream for {StreamUrl} {streamName}: {ErrorMessage}", streamUrl, streamName, error?.Message);
        }
    }

    private (Stream? stream, int processId, ProxyStreamError? error) GetCommandStream(string streamUrl, string Command, string Parameters, string clientUserAgent)
    {

        string commandExec = Path.Combine(BuildInfo.AppDataFolder, Command);

        if (!File.Exists(commandExec) && !File.Exists(commandExec + ".exe"))
        {
            if (!IsCommandAvailable(Command))
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"Executable file not found: {Command}" };
                return (null, -1, error);
            }
            commandExec = Command;
        }

        try
        {
            return CreateCommandStream(commandExec, Parameters, streamUrl, clientUserAgent);
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
    private (Stream? stream, int processId, ProxyStreamError? error) CreateCommandStream(
    string commandExec,
    string parameters,
    string streamUrl,
    string clientUserAgent,
    CancellationToken cancellationToken = default)
    {

        try
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string options = parameters.Replace("{streamUrl}", $"\"{streamUrl}\"") + $" -user_agent \"{clientUserAgent}\"";

            using Process process = new();
            ConfigureProcess(process, commandExec, options);


            cancellationToken.ThrowIfCancellationRequested();

            bool processStarted = process.Start();
            stopwatch.Stop();

            if (!processStarted)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start process" };
                logger.LogError("CreateCommandStream Error: {ErrorMessage}", error.Message);
                return (null, -1, error);
            }

            logger.LogInformation("Opened stream with args \"{formattedArgs}\" in {ElapsedMilliseconds} ms", options, stopwatch.ElapsedMilliseconds);


            //Task.Run(() =>
            //{
            //    //// Read standard error to avoid process blocking due to full buffer
            //    //string? errorOutput = process?.StandardError?.ReadToEnd() ?? null;
            //    //if (!string.IsNullOrEmpty(errorOutput))
            //    //{
            //    //    logger.LogError("Process StandardError: {ErrorOutput}", errorOutput);
            //    //}
            //}, cancellationToken);

            return (process.StandardOutput.BaseStream, process.Id, null);
        }
        catch (OperationCanceledException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.OperationCancelled, Message = "Operation was cancelled" };
            logger.LogError(ex, "CreateCommandStream Error: {ErrorMessage}", error.Message);
            return (null, -1, error);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "CreateCommandStream Error: {ErrorMessage}", error.Message);
            return (null, -1, error);
        }
    }

    private static void ConfigureProcess(Process process, string commandExec, string formattedArgs)
    {
        process.StartInfo.FileName = commandExec;
        process.StartInfo.Arguments = formattedArgs;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
    }

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

    ////public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";
    //private async Task<(Stream? stream, int processId, ProxyStreamError? error)> CreateFFMpegStream(string ffmpegExec, string streamUrl, string streamName)
    //{
    //    Setting settings = intSettings.CurrentValue;

    //    try
    //    {
    //        Stopwatch stopwatch = Stopwatch.StartNew();

    //        string options = string.IsNullOrEmpty(FFMpegOptions) ? BuildInfo.FFMPEGDefaultOptions : FFMpegOptions;

    //        string formattedArgs = options.Replace("{streamUrl}", $"\"{streamUrl}\"");
    //        formattedArgs += $" -user_agent \"{settings.StreamingClientUserAgent}\"";

    //        using Process process = new();
    //        process.StartInfo.FileName = ffmpegExec;
    //        process.StartInfo.Arguments = formattedArgs;
    //        process.StartInfo.CreateNoWindow = true;
    //        process.StartInfo.UseShellExecute = false;
    //        process.StartInfo.RedirectStandardOutput = true;
    //        process.StartInfo.RedirectStandardError = true;

    //        bool processStarted = process.Start();
    //        stopwatch.Stop();
    //        if (!processStarted)
    //        {
    //            // Log and return an error if the process couldn't be started
    //            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
    //            logger.LogError("CreateFFMpegStream Error: {ErrorMessage}", error.Message);

    //            return (null, -1, error);
    //        }

    //        // Return the standard output stream of the process

    //        logger.LogInformation("Opened ffmpeg stream for {streamName} with args \"{formattedArgs}\" in {ElapsedMilliseconds} ms", streamName, formattedArgs, stopwatch.ElapsedMilliseconds);
    //        return (await Task.FromResult(process.StandardOutput.BaseStream).ConfigureAwait(false), process.Id, null);
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log and return any exceptions that occur
    //        ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
    //        logger.LogError(ex, "CreateFFMpegStream Error: {ErrorMessage}", error.Message);
    //        return (null, -1, error);
    //    }
    //}

    private (Stream? stream, int processId, ProxyStreamError? error) HandleFFMpegStreamException<T>(ProxyStreamErrorCode errorCode, T exception) where T : Exception
    {
        ProxyStreamError error = new() { ErrorCode = errorCode, Message = exception.Message };
        logger.LogError(exception, "GetFFMpegStream Error: {message}", error.Message);
        return (null, -1, error);
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(SMStreamDto smStream, string clientUserAgent, CancellationToken cancellationToken)
    {
        Setting settings = intSettings.CurrentValue;

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            if (smStream.Url.EndsWith(".m3u8"))
            {
                string? ffmpeg = GetFFPMpegExec();
                if (ffmpeg == null)
                {
                    logger.LogCritical("FFMPEG not found");
                    return (null, -1, new ProxyStreamError() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = "FFMPEG not found" });
                }
                logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl} {streamName}", smStream.Url, smStream.Name);
                return GetCommandStream(smStream.Url, ffmpeg, FFMpegOptions, clientUserAgent);
            }

            HttpClient client = CreateHttpClient(settings.StreamingClientUserAgent);
            HttpResponseMessage? response = await client.GetWithRedirectAsync(smStream.Url, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (response?.IsSuccessStatusCode != true)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = $"Could not retrieve stream for {smStream.Name} {response?.StatusCode}", };
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
                string? ffmpeg = GetFFPMpegExec();
                if (ffmpeg == null)
                {
                    logger.LogCritical("FFMPEG not found");
                    return (null, -1, new ProxyStreamError() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = "FFMPEG not found" });
                }

                logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl} {streamName}", smStream.Url, smStream.Name);

                return GetCommandStream(smStream.Url, ffmpeg, FFMpegOptions, clientUserAgent);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            stopwatch.Stop(); // Stop the stopwatch when the stream is retrieved
            logger.LogInformation("Opened stream for {streamName} in {ElapsedMilliseconds} ms", smStream.Name, stopwatch.ElapsedMilliseconds);

            return (stream, -1, null);
        }
        catch (Exception ex) when (ex is HttpRequestException or Exception)
        {
            stopwatch.Stop();
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = ex.Message };
            string message = $"GetProxyStream Error for {smStream.Name}";
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

    private static bool IsCommandAvailable(string proxyCommand)
    {
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        ProcessStartInfo startInfo = new(command, proxyCommand)
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

    //private static bool IsFFmpegAvailable()
    //{
    //    string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
    //    ProcessStartInfo startInfo = new(command, "ffmpeg")
    //    {
    //        RedirectStandardOutput = true,
    //        UseShellExecute = false
    //    };
    //    Process process = new()
    //    {
    //        StartInfo = startInfo
    //    };
    //    _ = process.Start();
    //    process.WaitForExit();
    //    return process.ExitCode == 0;
    //}
}
