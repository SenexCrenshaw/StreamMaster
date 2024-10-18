using StreamMaster.Domain.Extensions;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

public class HTTPStream(ILogger<HTTPStream> logger, IOptionsMonitor<Setting> _settings, IHttpClientFactory httpClientFactory, IProfileService profileService, ICommandExecutor commandExecutor) : IHTTPStream
{
    private HttpClient CreateHttpClient(string streamingClientUserAgent)
    {
        HttpClient client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(streamingClientUserAgent);
        return client;
    }

    public async Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        CancellationTokenSource? timeoutCts = null;
        logger.LogInformation("Getting stream for {streamName}", smStreamInfo.Name);
        try
        {
            if (_settings.CurrentValue.StreamStartTimeoutMs > 0)
            {
                timeoutCts = new CancellationTokenSource(_settings.CurrentValue.StreamStartTimeoutMs);
            }

            using CancellationTokenSource linkedCts = timeoutCts != null
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token)
                : CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            HttpClient client = CreateHttpClient(clientUserAgent);
            HttpResponseMessage? response;

            try
            {
                response = await client.GetWithRedirectAsync(smStreamInfo.Url, cancellationToken: linkedCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (timeoutCts?.IsCancellationRequested == true)
            {
                ProxyStreamError timeoutError = new() { ErrorCode = ProxyStreamErrorCode.Timeout, Message = $"Request for {smStreamInfo.Name} timed out after {_settings.CurrentValue.StreamStartTimeoutMs} ms" };
                logger.LogError("HandleStream Timeout: {message}", timeoutError.Message);
                return (null, -1, timeoutError);
            }

            if (response?.IsSuccessStatusCode != true)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = $"Could not retrieve stream for {smStreamInfo.Name}. Response was: \"{response?.StatusCode}\"" };
                logger.LogError("GetProxyStream Error: {message}", error.Message);
                return (null, -1, error);
            }

            string? contentType = response.Content.Headers?.ContentType?.MediaType;

            if (!string.IsNullOrEmpty(contentType) &&
                (contentType.Equals("application/vnd.apple.mpegurl", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("audio/mpegurl", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("application/x-mpegURL", StringComparison.OrdinalIgnoreCase)))
            {
                CommandProfileDto commandProfileDto = profileService.GetM3U8OutputProfile(smStreamInfo.Id);
                logger.LogInformation("Stream contains HLS content, using {name} for streaming: {streamName}", commandProfileDto.ProfileName, smStreamInfo.Name);
                //CommandProfileDto commandProfileDto = profileService.GetCommandProfile("SMFFMPEG");
                stopwatch.Stop();
                logger.LogInformation("Got stream for {streamName} in {ElapsedMilliseconds} ms", smStreamInfo.Name, stopwatch.ElapsedMilliseconds);

                return commandExecutor.ExecuteCommand(commandProfileDto, smStreamInfo.Url, clientUserAgent, null, linkedCts.Token);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(linkedCts.Token).ConfigureAwait(false);
            stopwatch.Stop();
            logger.LogInformation("Got stream for {streamName} in {ElapsedMilliseconds} ms", smStreamInfo.Name, stopwatch.ElapsedMilliseconds);

            return (stream, -1, null);
        }
        catch (OperationCanceledException) when (timeoutCts?.IsCancellationRequested == true)
        {
            ProxyStreamError timeoutError = new() { ErrorCode = ProxyStreamErrorCode.Timeout, Message = $"Request for {smStreamInfo.Name} timed out after {_settings.CurrentValue.StreamStartTimeoutMs} ms" };
            logger.LogError("HandleStream Timeout: {message}", timeoutError.Message);
            return (null, -1, timeoutError);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.DownloadError, Message = $"Unexpected error occurred while handling stream for {smStreamInfo.Name}: {ex.Message}" };
            logger.LogError(ex, "HandleStream Error: {message}", error.Message);
            return (null, -1, error);
        }
        finally
        {
            stopwatch.Stop();
            timeoutCts?.Dispose();
        }
    }
}