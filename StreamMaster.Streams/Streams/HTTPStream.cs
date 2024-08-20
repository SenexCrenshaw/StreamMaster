using StreamMaster.Domain.Extensions;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class HTTPStream(ILogger<HTTPStream> logger, IHttpClientFactory httpClientFactory, IProfileService profileService, ICommandExecutor commandExecutor) : IHTTPStream
{
    private HttpClient CreateHttpClient(string streamingClientUserAgent)
    {
        HttpClient client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(streamingClientUserAgent);
        return client;
    }

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(SMStreamInfo sMStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {

        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            HttpClient client = CreateHttpClient(clientUserAgent);
            HttpResponseMessage? response = await client.GetWithRedirectAsync(sMStreamInfo.Url, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (response?.IsSuccessStatusCode != true)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = $"Could not retrieve stream for {sMStreamInfo.Name} Response Was: \"{response?.StatusCode}\"" };
                logger.LogError("GetProxyStream Error: {message}", error.Message);
                return (null, -1, error);
            }

            string? contentType = response.Content.Headers?.ContentType?.MediaType;

            if (!string.IsNullOrEmpty(contentType) &&
                (contentType.Equals("application/vnd.apple.mpegurl", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("audio/mpegurl", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("application/x-mpegURL", StringComparison.OrdinalIgnoreCase)))
            {
                logger.LogInformation("Stream contains HLS content, using ffmpeg for streaming: {streamName}", sMStreamInfo.Name);
                CommandProfileDto profile = profileService.GetCommandProfile("SMFFMPEG");
                return commandExecutor.ExecuteCommand(profile, sMStreamInfo.Url, clientUserAgent, null, cancellationToken);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            logger.LogInformation("Opened stream for {streamName} in {ElapsedMilliseconds} ms", sMStreamInfo.Name, stopwatch.ElapsedMilliseconds);

            return (stream, -1, null);
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
