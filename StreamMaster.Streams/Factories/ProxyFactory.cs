using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

using System.Diagnostics;
namespace StreamMaster.Streams.Factories;

public sealed class ProxyFactory(ILogger<ProxyFactory> logger, IHttpClientFactory httpClientFactory, IOptionsMonitor<Setting> intSettings, IOptionsMonitor<VideoOutputProfiles> intProfileSettings)
    : IProxyFactory
{
    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -user_agent {clientUserAgent} -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1";
    public string CustomPlayListFFMpegOptions { get; set; } = "-hide_banner -loglevel error -re -i {streamUrl} -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1";

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        Stream? stream;
        ProxyStreamError? error;
        int processId;

        (stream, processId, error) = await GetProxyStream(channelStatus, cancellationToken);
        LogErrorIfAny(stream, error, channelStatus.SMStream.Url, channelStatus.SMStream.Name);
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
        string? exec = FileUtil.GetExec(Command);

        if (string.IsNullOrEmpty(exec))
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"Executable file not found: {Command}" };
            return (null, -1, error);
        }

        try
        {
            return CreateCommandStream(exec, Parameters, streamUrl, clientUserAgent);
        }
        catch (IOException ex)
        {
            return HandleStreamException(ProxyStreamErrorCode.IoError, ex);
        }
        catch (Exception ex)
        {
            return HandleStreamException(ProxyStreamErrorCode.UnknownError, ex);
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

            string options = parameters.Replace("{streamUrl}", $"\"{streamUrl}\"");
            options = options.Replace("{clientUserAgent}", $"\"{clientUserAgent}\"");

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

    private (Stream? stream, int processId, ProxyStreamError? error) HandleStreamException<T>(ProxyStreamErrorCode errorCode, T exception) where T : Exception
    {
        ProxyStreamError error = new() { ErrorCode = errorCode, Message = exception.Message };
        logger.LogError(exception, "GetProxy Error: {message}", error.Message);
        return (null, -1, error);
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Setting settings = intSettings.CurrentValue;

            string clientUserAgent = settings.StreamingClientUserAgent;
            if (!string.IsNullOrEmpty(channelStatus.SMStream.ClientUserAgent))
            {
                clientUserAgent = channelStatus.SMStream.ClientUserAgent;
            }

            if (channelStatus.SMChannel.IsCustomStream)
            {
                //var customPlayList = customPlayListBuilder.GetCustomPlayList(smStream.Name);
                if (channelStatus.CustomPlayList == null)
                {
                    ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"Custom playlist not found: {channelStatus.SMChannel.Name}" };
                    logger.LogError("GetProxyStream Error: {message}", error.Message);
                    return (null, -1, error);
                }

                string? exec = FileUtil.GetExec(channelStatus.VideoProfile.Command);
                if (exec == null)
                {
                    logger.LogCritical("Profile {profileName} Command {command} not found", channelStatus.VideoProfile.ProfileName, channelStatus.VideoProfile.Command);
                    return (null, -1, new ProxyStreamError() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = "FFMPEG not found" });
                }
                return GetCommandStream(channelStatus.CustomPlayList.CustomStreamNfos[channelStatus.CurrentRank].VideoFileName, "ffmpeg", CustomPlayListFFMpegOptions, clientUserAgent);
            }
            SMStreamDto smStream = channelStatus.SMStream;

            if (smStream.Url.EndsWith(".m3u8"))
            {
                string? exec = FileUtil.GetExec(channelStatus.VideoProfile.Command);

                if (exec == null)
                {
                    logger.LogCritical("Profile {profileName} Command {command} not found", channelStatus.VideoProfile.ProfileName, channelStatus.VideoProfile.Command);
                    return (null, -1, new ProxyStreamError() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = "FFMPEG not found" });
                }
                logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl} {streamName}", smStream.Url, smStream.Name);
                return GetCommandStream(smStream.Url, exec, channelStatus.VideoProfile.Parameters, clientUserAgent);
            }

            if (channelStatus.VideoProfile.ProfileName != "StreamMaster")
            {
                string? exec = FileUtil.GetExec(channelStatus.VideoProfile.Command);

                if (exec == null)
                {
                    logger.LogCritical("Profile {profileName} {command} not found", channelStatus.VideoProfile.ProfileName, channelStatus.VideoProfile.Command);
                    return (null, -1, new ProxyStreamError() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = "FFMPEG not found" });
                }

                logger.LogInformation("Using {command} for streaming: {streamName}", channelStatus.VideoProfile.Command, smStream.Name);

                return GetCommandStream(smStream.Url, exec, channelStatus.VideoProfile.Parameters, clientUserAgent);
            }

            HttpClient client = CreateHttpClient(clientUserAgent);
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

                if (intProfileSettings.CurrentValue.VideoProfiles.TryGetValue("FFMPEG", out VideoOutputProfile? ffmpegProfile))
                {
                    logger.LogInformation("Stream URL has HLS content, using {command} for streaming: {streamName}", ffmpegProfile.Command, smStream.Name);
                    return GetCommandStream(smStream.Url, ffmpegProfile.Command, ffmpegProfile.Parameters, clientUserAgent);
                }

                logger.LogInformation("Stream URL has HLS content, using ffmpeg for streaming: {streamName}", smStream.Name);

                return GetCommandStream(smStream.Url, "ffmpeg", FFMpegOptions, clientUserAgent);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Opened stream for {streamName} in {ElapsedMilliseconds} ms", smStream.Name, stopwatch.ElapsedMilliseconds);

            return (stream, -1, null);
        }
        catch (Exception ex) when (ex is HttpRequestException or Exception)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = ex.Message };
            string message = $"GetProxyStream Error for {channelStatus.SMStream.Name}";
            logger.LogError(ex, message, error.Message);
            return (null, -1, error);
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private HttpClient CreateHttpClient(string streamingClientUserAgent)
    {
        HttpClient client = httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.UserAgent.ParseAdd(streamingClientUserAgent);
        return client;
    }

    //private static bool IsCommandAvailable(string proxyCommand)
    //{
    //    string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
    //    ProcessStartInfo startInfo = new(command, proxyCommand)
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
