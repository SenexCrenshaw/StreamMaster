using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
using StreamMaster.PlayList;

using System.Diagnostics;

namespace StreamMaster.Streams.Factories;

public sealed class ProxyFactory(
    ILogger<ProxyFactory> logger,
    ICustomPlayListBuilder customPlayListBuilder,
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<Setting> settings,
    IOptionsMonitor<CommandProfileList> profileSettings, ICommandExecutor commandExecutor) : IProxyFactory
{
    private readonly IOptionsMonitor<Setting> _settings = settings;

    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -user_agent {clientUserAgent} -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1";
    public string CustomPlayListFFMpegOptions { get; set; } = "-hide_banner -loglevel error -ss {secondsIn} -re -i {streamUrl} -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1";

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await GetProxyStream(channelStatus, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null)
        {
            logger.LogError("Error getting proxy stream for {streamName}: {ErrorMessage}", channelStatus.SMStream.Name, error?.Message);
        }
        return (stream, processId, error);
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxyStream(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            string clientUserAgent = GetClientUserAgent(channelStatus);

            return channelStatus.SMChannel.IsCustomStream
                ? HandleCustomStream(channelStatus, clientUserAgent, cancellationToken)
                : await HandleRegularStream(channelStatus, clientUserAgent, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is HttpRequestException or Exception)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = ex.Message };
            logger.LogError(ex, "GetProxyStream Error for {channelStatus.SMStream.Name}", error.Message);
            return (null, -1, error);
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private string GetClientUserAgent(IChannelStatus channelStatus)
    {
        Setting settings = _settings.CurrentValue;
        return !string.IsNullOrEmpty(channelStatus.SMStream.ClientUserAgent) ? channelStatus.SMStream.ClientUserAgent : settings.SourceClientUserAgent;
    }

    private (Stream? stream, int processId, ProxyStreamError? error) HandleCustomStream(IChannelStatus channelStatus, string clientUserAgent, CancellationToken cancellationToken)
    {
        if (channelStatus.CustomPlayList == null)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"Custom playlist not found: {channelStatus.SMChannel.Name}" };
            logger.LogError("GetProxyStream Error: {message}", error.Message);
            return (null, -1, error);
        }

        (string videoFileName, int secondsIn) = GetCustomStreamFileName(channelStatus);
        string options = CustomPlayListFFMpegOptions.Replace("{secondsIn}", secondsIn.ToString());
        return commandExecutor.ExecuteCommand("ffmpeg", options, channelStatus.SMStream.Id, videoFileName, clientUserAgent, cancellationToken);
    }

    private (string videoFileName, int secondsIn) GetCustomStreamFileName(IChannelStatus channelStatus)
    {
        CustomStreamNfo? intro = customPlayListBuilder.GetIntro(channelStatus.IntroIndex);
        if (intro != null && !channelStatus.PlayedIntro)
        {
            channelStatus.PlayedIntro = true;
            channelStatus.IntroIndex++;
            if (channelStatus.IntroIndex >= customPlayListBuilder.IntroCount)
            {
                channelStatus.IntroIndex = 0;
            }

            return (intro.VideoFileName, 0);
        }

        channelStatus.PlayedIntro = false;
        if (channelStatus.IsFirst)
        {
            channelStatus.IsFirst = false;
            int secondsIn;
            (CustomStreamNfo StreamNfo, secondsIn) = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(channelStatus.CustomPlayList.Name);
            return (StreamNfo.VideoFileName, secondsIn);
        }
        else
        {
            (CustomStreamNfo StreamNfo, _) = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(channelStatus.CustomPlayList.Name);
            return (StreamNfo.VideoFileName, 0);
        }
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleRegularStream(IChannelStatus channelStatus, string clientUserAgent, CancellationToken cancellationToken)
    {
        if (channelStatus.SMStream.Url.EndsWith(".m3u8"))
        {
            logger.LogInformation("Stream URL has HLS content, using FFMpeg for streaming: {StreamUrl} {streamName}", channelStatus.SMStream.Url, channelStatus.SMStream.Name);
            return commandExecutor.ExecuteCommand(channelStatus.CommandProfile, channelStatus.SMStream.Url, clientUserAgent, cancellationToken);
        }

        if (channelStatus.CommandProfile.ProfileName != "StreamMaster")
        {
            logger.LogInformation("Using {command} for streaming: {streamName}", channelStatus.CommandProfile.Command, channelStatus.SMStream.Name);
            return commandExecutor.ExecuteCommand(channelStatus.CommandProfile, channelStatus.SMStream.Url, clientUserAgent, cancellationToken);
        }

        return await HandleHttpStream(channelStatus.SMStream, channelStatus.CommandProfile.ProfileName, clientUserAgent, cancellationToken).ConfigureAwait(false);
    }

    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleHttpStream(SMStreamDto smStream, string profileName, string clientUserAgent, CancellationToken cancellationToken)
    {
        HttpClient client = CreateHttpClient(clientUserAgent);
        HttpResponseMessage? response = await client.GetWithRedirectAsync(smStream.Url, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (response?.IsSuccessStatusCode != true)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = $"Could not retrieve stream for {smStream.Name} {response?.StatusCode}" };
            logger.LogError("GetProxyStream Error: {message}", error.Message);
            return (null, -1, error);
        }

        string? contentType = response.Content.Headers?.ContentType?.MediaType;

        if (!string.IsNullOrEmpty(contentType) &&
            (contentType.Equals("application/vnd.apple.mpegurl", StringComparison.OrdinalIgnoreCase) ||
            contentType.Equals("audio/mpegurl", StringComparison.OrdinalIgnoreCase) ||
            contentType.Equals("application/x-mpegURL", StringComparison.OrdinalIgnoreCase)))
        {
            if (profileSettings.CurrentValue.CommandProfiles.TryGetValue("FFMPEG", out CommandProfile? ffmpegProfile))
            {
                logger.LogInformation("Stream URL has HLS content, using {command} for streaming: {streamName}", ffmpegProfile.Command, smStream.Name);
                return commandExecutor.ExecuteCommand(ffmpegProfile.Command, ffmpegProfile.Parameters, profileName, smStream.Url, clientUserAgent, cancellationToken);
                //return GetCommandStream(smStream.Url, ffmpegProfile.Command, ffmpegProfile.Parameters, clientUserAgent);
            }

            logger.LogInformation("Stream URL has HLS content, using ffmpeg for streaming: {streamName}", smStream.Name);
            return commandExecutor.ExecuteCommand("ffmpeg", FFMpegOptions, profileName, smStream.Url, clientUserAgent, cancellationToken);
        }

        Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Opened stream for {streamName} in {ElapsedMilliseconds} ms", smStream.Name, Stopwatch.GetTimestamp());

        return (stream, -1, null);
    }

    //private (Stream? stream, int processId, ProxyStreamError? error) GetCommandStream(string streamUrl, string command, string parameters, string clientUserAgent)
    //{
    //    string? exec = FileUtil.GetExec(command);

    //    if (string.IsNullOrEmpty(exec))
    //    {
    //        ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"Executable file not found: {command}" };
    //        return (null, -1, error);
    //    }

    //    try
    //    {
    //        return CreateCommandStream(exec, parameters, streamUrl, clientUserAgent);
    //    }
    //    catch (IOException ex)
    //    {
    //        return HandleStreamException(ProxyStreamErrorCode.IoError, ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        return HandleStreamException(ProxyStreamErrorCode.UnknownError, ex);
    //    }
    //}

    //private (Stream? stream, int processId, ProxyStreamError? error) CreateCommandStream(
    //    string commandExec,
    //    string parameters,
    //    string streamUrl,
    //    string clientUserAgent,
    //    CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        Stopwatch stopwatch = Stopwatch.StartNew();

    //        string options = parameters.Replace("{streamUrl}", $"\"{streamUrl}\"").Replace("{clientUserAgent}", $"\"{clientUserAgent}\"");

    //        using Process process = new();
    //        ConfigureProcess(process, commandExec, options);
    //        cancellationToken.ThrowIfCancellationRequested();

    //        bool processStarted = process.Start();
    //        stopwatch.Stop();

    //        if (!processStarted)
    //        {
    //            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start process" };
    //            logger.LogError("CreateCommandStream Error: {ErrorMessage}", error.Message);
    //            return (null, -1, error);
    //        }

    //        logger.LogInformation("Opened stream with args \"{formattedArgs}\" in {ElapsedMilliseconds} ms", options, stopwatch.ElapsedMilliseconds);
    //        return (process.StandardOutput.BaseStream, process.Id, null);
    //    }
    //    catch (OperationCanceledException ex)
    //    {
    //        ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.OperationCancelled, Message = "Operation was cancelled" };
    //        logger.LogError(ex, "CreateCommandStream Error: {ErrorMessage}", error.Message);
    //        return (null, -1, error);
    //    }
    //    catch (Exception ex)
    //    {
    //        ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
    //        logger.LogError(ex, "CreateCommandStream Error: {ErrorMessage}", error.Message);
    //        return (null, -1, error);
    //    }
    //}

    //private static void ConfigureProcess(Process process, string commandExec, string formattedArgs)
    //{
    //    process.StartInfo.FileName = commandExec;
    //    process.StartInfo.Arguments = formattedArgs;
    //    process.StartInfo.CreateNoWindow = true;
    //    process.StartInfo.UseShellExecute = false;
    //    process.StartInfo.RedirectStandardOutput = true;
    //    process.StartInfo.RedirectStandardError = true;
    //}

    //private (Stream? stream, int processId, ProxyStreamError? error) HandleStreamException<T>(ProxyStreamErrorCode errorCode, T exception) where T : Exception
    //{
    //    ProxyStreamError error = new() { ErrorCode = errorCode, Message = exception.Message };
    //    logger.LogError(exception, "GetProxy Error: {message}", error.Message);
    //    return (null, -1, error);
    //}

    private HttpClient CreateHttpClient(string streamingClientUserAgent)
    {
        HttpClient client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(streamingClientUserAgent);
        return client;
    }
}
