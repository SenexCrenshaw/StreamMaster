using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class HTTPStream(ILogger<HTTPStream> logger, IOptionsMonitor<CommandProfileList> profileSettings, IHttpClientFactory httpClientFactory, ICommandExecutor commandExecutor) : IHTTPStream
{
    public const string FFMpegOptions = "-hide_banner -loglevel error -user_agent {clientUserAgent} -i {streamUrl} -reconnect 1 -map 0:v -map 0:a? -map 0:s? -c copy -f mpegts pipe:1";

    private HttpClient CreateHttpClient(string streamingClientUserAgent)
    {
        HttpClient client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(streamingClientUserAgent);
        return client;
    }

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(IChannelStatus channelStatus, string clientUserAgent, CancellationToken cancellationToken)
    {
        SMStreamDto smStream = channelStatus.SMStream;
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
                return commandExecutor.ExecuteCommand(ffmpegProfile.Command, ffmpegProfile.Parameters, "FFMPEG", smStream.Url, clientUserAgent, cancellationToken);
                //return GetCommandStream(smStream.Url, ffmpegProfile.Command, ffmpegProfile.Parameters, clientUserAgent);
            }

            logger.LogInformation("Stream URL has HLS content, using ffmpeg for streaming: {streamName}", smStream.Name);
            return commandExecutor.ExecuteCommand("ffmpeg", FFMpegOptions, "FFMPEG", smStream.Url, clientUserAgent, cancellationToken);
        }

        Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInformation("Opened stream for {streamName} in {ElapsedMilliseconds} ms", smStream.Name, Stopwatch.GetTimestamp());

        return (stream, -1, null);
    }


}
