using System.Diagnostics;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.Streams.Streams;

/// <summary>
/// Handles the creation and management of HTTP-based streams.
/// </summary>
public class HTTPStream(
    ILogger<HTTPStream> logger,
    IOptionsMonitor<Setting> settings,
    IHttpClientFactory httpClientFactory,
    IProfileService profileService,
    ICommandExecutor commandExecutor) : IHTTPStream
{
    /// <summary>
    /// Creates an `HttpClient` configured with the specified user agent.
    /// </summary>
    /// <param name="streamingClientUserAgent">The user agent to use for the client.</param>
    /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
    private HttpClient CreateHttpClient(string streamingClientUserAgent)
    {
        HttpClient client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(streamingClientUserAgent);
        return client;
    }

    /// <inheritdoc/>
    public async Task<GetStreamResult> HandleStream(SMStreamInfo smStreamInfo, string clientUserAgent, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        CancellationTokenSource? timeoutCts = null;

        logger.LogInformation("Getting stream for {StreamName}", smStreamInfo.Name);

        try
        {
            if (settings.CurrentValue.StreamStartTimeoutMs > 0)
            {
                timeoutCts = new CancellationTokenSource(settings.CurrentValue.StreamStartTimeoutMs);
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
                return CreateTimeoutResult(smStreamInfo, settings.CurrentValue.StreamStartTimeoutMs);
            }

            if (response?.IsSuccessStatusCode != true)
            {
                return CreateErrorResult(smStreamInfo, response?.StatusCode.ToString() ?? "Unknown response");
            }

            string? contentType = response.Content.Headers?.ContentType?.MediaType;

            if (IsHLSContent(contentType))
            {
                return HandleHLSContent(smStreamInfo, clientUserAgent, stopwatch, linkedCts.Token);
            }

            Stream stream = await response.Content.ReadAsStreamAsync(linkedCts.Token).ConfigureAwait(false);
            stopwatch.Stop();
            logger.LogInformation("Got stream for {StreamName} in {ElapsedMilliseconds} ms", smStreamInfo.Name, stopwatch.ElapsedMilliseconds);

            return new GetStreamResult(stream, -1, null);
        }
        catch (OperationCanceledException) when (timeoutCts?.IsCancellationRequested == true)
        {
            return CreateTimeoutResult(smStreamInfo, settings.CurrentValue.StreamStartTimeoutMs);
        }
        catch (Exception ex)
        {
            return HandleUnexpectedError(smStreamInfo, ex);
        }
        finally
        {
            stopwatch.Stop();
            timeoutCts?.Dispose();
        }
    }

    private static bool IsHLSContent(string? contentType)
    {
        return !string.IsNullOrEmpty(contentType) &&
               (contentType.EqualsIgnoreCase("application/vnd.apple.mpegurl") ||
                contentType.EqualsIgnoreCase("audio/mpegurl") ||
                contentType.EqualsIgnoreCase("application/x-mpegURL"));
    }

    private GetStreamResult HandleHLSContent(SMStreamInfo smStreamInfo, string clientUserAgent, Stopwatch stopwatch, CancellationToken cancellationToken)
    {
        CommandProfileDto commandProfileDto = profileService.GetM3U8OutputProfile(smStreamInfo.Id);

        logger.LogInformation("Stream contains HLS content, using {ProfileName} for streaming: {StreamName}", commandProfileDto.ProfileName, smStreamInfo.Name);

        stopwatch.Stop();
        logger.LogInformation("Got HLS stream for {StreamName} in {ElapsedMilliseconds} ms", smStreamInfo.Name, stopwatch.ElapsedMilliseconds);

        return commandExecutor.ExecuteCommand(commandProfileDto, smStreamInfo.Url, clientUserAgent, null, cancellationToken);
    }

    private static GetStreamResult CreateTimeoutResult(SMStreamInfo smStreamInfo, int timeoutMs)
    {
        ProxyStreamError timeoutError = new()
        {
            ErrorCode = ProxyStreamErrorCode.Timeout,
            Message = $"Request for {smStreamInfo.Name} timed out after {timeoutMs} ms"
        };

        return new GetStreamResult(null, -1, timeoutError);
    }

    private static GetStreamResult CreateErrorResult(SMStreamInfo smStreamInfo, string responseDescription)
    {
        ProxyStreamError error = new()
        {
            ErrorCode = ProxyStreamErrorCode.DownloadError,
            Message = $"Could not retrieve stream for {smStreamInfo.Name}. Response: \"{responseDescription}\""
        };

        return new GetStreamResult(null, -1, error);
    }

    private GetStreamResult HandleUnexpectedError(SMStreamInfo smStreamInfo, Exception ex)
    {
        ProxyStreamError error = new()
        {
            ErrorCode = ProxyStreamErrorCode.DownloadError,
            Message = $"Unexpected error occurred while handling stream for {smStreamInfo.Name}: {ex.Message}"
        };

        logger.LogError(ex, "Unexpected error occurred for {StreamName}: {Message}", smStreamInfo.Name, error.Message);

        return new GetStreamResult(null, -1, error);
    }
}
