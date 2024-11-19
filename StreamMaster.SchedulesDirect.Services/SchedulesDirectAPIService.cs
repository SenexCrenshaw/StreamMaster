using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.SchedulesDirect;

public partial class SchedulesDirectAPIService : ISchedulesDirectAPIService
{
    private readonly ILogger<SchedulesDirectAPIService> logger;
    private readonly SDSettings sdsettings;
    private readonly Setting settings;
    public HttpClient _httpClient = null!;

    private const string BaseAddress = "https://json.schedulesdirect.org/20141201/";

    public SchedulesDirectAPIService(ILogger<SchedulesDirectAPIService> logger, IOptionsMonitor<SDSettings> intsdsettings, IOptionsMonitor<Setting> intSettings)
    {
        this.logger = logger;
        sdsettings = intsdsettings.CurrentValue;
        settings = intSettings.CurrentValue;
        CreateHttpClient();
    }

    private async Task<List<ProgramMetadata>?> GetArtworkAsync(string[] request)
    {
        DateTime dtStart = DateTime.Now;
        List<ProgramMetadata>? ret = await GetApiResponse<List<ProgramMetadata>>(APIMethod.POST, "metadata/programs/", request);
        if (ret != null)
        {
            logger.LogDebug($"Successfully retrieved artwork info for {ret.Count}/{request.Length} programs. ({DateTime.Now - dtStart:G})");
        }
        else
        {
            logger.LogDebug($"Did not receive a response from Schedules Direct for artwork info of {request.Length} programs. ({DateTime.Now - dtStart:G})");
        }

        return ret;
    }

    public async Task DownloadImageResponsesAsync(List<string> imageQueue, ConcurrentBag<ProgramMetadata> metadata, int start = 0)
    {
        // Reject 0 requests
        if (imageQueue.Count - start < 1)
        {
            return;
        }

        // Build the array of series to request images for
        string[] series = new string[Math.Min(imageQueue.Count - start, SchedulesDirect.MaxImgQueries)];
        for (int i = 0; i < series.Length; ++i)
        {
            series[i] = imageQueue[start + i];
        }

        // Request images from ScheduleService Direct
        List<ProgramMetadata>? responses = await GetArtworkAsync(series).ConfigureAwait(false);
        if (responses != null)
        {
            foreach (ProgramMetadata response in responses)
            {
                metadata.Add(response);
            }
        }
        else
        {
            logger.LogInformation("Did not receive a response from Schedules Direct for artwork info of {count} programs, first entry {entry}.", series.Length, series.Length != 0 ? series[0] : "");
        }
    }

    public async Task<HttpResponseMessage?> GetSdImage(string uri)//, DateTimeOffset ifModifiedSince)
    {
        try
        {
            HttpRequestMessage message = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{BaseAddress}{uri}")
            };
            HttpResponseMessage response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return !response.IsSuccessStatusCode || response.Content?.Headers?.ContentType?.MediaType == "application/json"
                ? response.Content?.Headers?.ContentType?.MediaType == "application/json"
                    ? await HandleHttpResponseError(response, await response.Content.ReadAsStringAsync())
                    : await HandleHttpResponseError(response, null)
                : response;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError("{uri} GetSdImage() Exception: {Message}", uri, ex?.InnerException?.Message ?? ex?.Message);
        }
        return null;
    }

    private async Task<HttpResponseMessage> HandleHttpResponseError(HttpResponseMessage response, string? content)
    {
        string? tokenUsed = null;

        if (response.RequestMessage?.Headers.Contains("token") == true)
        {
            tokenUsed = response.RequestMessage.Headers.GetValues("token")?.FirstOrDefault();
        }

        if (!string.IsNullOrEmpty(content))
        {
            BaseResponse? err = JsonSerializer.Deserialize<BaseResponse>(content);
            if (err != null)
            {
                SDHttpResponseCode sdCode = (SDHttpResponseCode)err.Code;
                if (sdCode == SDHttpResponseCode.TOKEN_INVALID)
                {
                    logger.LogError("SDToken is invalid {token} {length}", tokenUsed, (tokenUsed?.Length) ?? 0);
                }
                switch (sdCode)
                {
                    case SDHttpResponseCode.SERVICE_OFFLINE: // SERVICE_OFFLINE
                        response.StatusCode = HttpStatusCode.ServiceUnavailable; // 503
                        response.ReasonPhrase = "Service Unavailable";
                        break;

                    case SDHttpResponseCode.ACCOUNT_DISABLED: // ACCOUNT_EXPIRED
                    case SDHttpResponseCode.ACCOUNT_EXPIRED: // ACCOUNT_DISABLED
                    case SDHttpResponseCode.APPLICATION_DISABLED: // APPLICATION_DISABLED
                        response.StatusCode = HttpStatusCode.Forbidden; // 403
                        response.ReasonPhrase = "Forbidden";
                        break;

                    case SDHttpResponseCode.ACCOUNT_LOCKOUT: // ACCOUNT_LOCKOUT
                        response.StatusCode = HttpStatusCode.Locked; // 423
                        response.ReasonPhrase = "Locked";
                        break;

                    case SDHttpResponseCode.IMAGE_NOT_FOUND: // IMAGE_NOT_FOUND
                    case SDHttpResponseCode.IMAGE_QUEUED: // IMAGE_QUEUED
                        response.StatusCode = HttpStatusCode.NotFound; // 404
                        response.ReasonPhrase = "Not Found";
                        break;

                    case SDHttpResponseCode.MAX_IMAGE_DOWNLOADS: // MAX_IMAGE_DOWNLOADS
                    case SDHttpResponseCode.MAX_IMAGE_DOWNLOADS_TRIAL: // MAX_IMAGE_DOWNLOADS_TRIAL
                        response.StatusCode = HttpStatusCode.TooManyRequests; // 429
                        response.ReasonPhrase = "Too Many Requests";
                        break;

                    case SDHttpResponseCode.TOKEN_MISSING: // TOKEN_MISSING - special case when token is getting refreshed due to below responses from a separate request
                    case SDHttpResponseCode.INVALID_USER: // INVALID_USER
                    case SDHttpResponseCode.TOKEN_INVALID:
                    case SDHttpResponseCode.TOKEN_EXPIRED: // TOKEN_EXPIRED
                    case SDHttpResponseCode.TOKEN_DUPLICATED: // TOKEN_DUPLICATED
                    case SDHttpResponseCode.UNKNOWN_USER: // UNKNOWN_USER
                        response.StatusCode = HttpStatusCode.Unauthorized; // 401
                        response.ReasonPhrase = "Unauthorized";
                        await ResetToken();
                        break;
                }
            }
        }

        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.StatusCode = (HttpStatusCode)418;
            response.ReasonPhrase = "I'm a teapot";
        }

        if (response.StatusCode != HttpStatusCode.NotModified)
        {
            string tokenUsedShort = tokenUsed?.Length >= 5 ? tokenUsed[..5] : tokenUsed ?? string.Empty;

            logger.LogError($"{response.RequestMessage?.RequestUri?.AbsolutePath.Replace(BaseAddress, "/")}: {(int)response.StatusCode} {response.ReasonPhrase} : Token={tokenUsedShort}...{(!string.IsNullOrEmpty(content) ? $"\n{content}" : "")}");
        }

        return response;
    }

    /// <summary>
    /// GetApiResponse
    /// </summary>
    /// <typeparam name="T">Object type to return.</typeparam>
    /// <param name="method">The http method to use.</param>
    /// <param name="uri">The relative uri from base address to form a complete url.</param>
    /// <param name="classObject">Payload of message to be serialized into json.</param>
    /// <returns>Object requested.</returns>
    public virtual async Task<T?> GetApiResponse<T>(APIMethod method, string uri, object? classObject = null, CancellationToken cancellationToken = default)
    {
        try
        {
            //await tokenSemaphore.WaitAsync(cancellationToken);

            //if (!GoodToken)
            //{
            CheckToken();
            //    if (!GoodToken)
            //    {
            //        return default;
            //    }
            //}

            switch (method)
            {
                case APIMethod.GET:
                    return await GetHttpResponse<T>(HttpMethod.Get, uri, cancellationToken: cancellationToken);

                case APIMethod.POST:
                    return await GetHttpResponse<T>(HttpMethod.Post, uri, classObject, cancellationToken: cancellationToken);

                case APIMethod.PUT:
                    return await GetHttpResponse<T>(HttpMethod.Put, uri, cancellationToken: cancellationToken);

                case APIMethod.DELETE:
                    return await GetHttpResponse<T>(HttpMethod.Delete, uri, cancellationToken: cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug("HTTP request exception thrown. Message:{FileUtil.ReportExceptionMessages}", FileUtil.ReportExceptionMessages(ex));
        }
        finally
        {
            //tokenSemaphore.Release();
        }
        return default;
    }

    private async Task<T?> GetHttpResponse<T>(HttpMethod method, string uri, object? content = null, CancellationToken cancellationToken = default)
    {
        string? json = null;

        try
        {
            using HttpRequestMessage request = new(method, uri)
            {
                Content = content != null
                    ? new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
                    : null
            };

            using HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                await HandleHttpResponseError(response, responseContent);
                return default;
            }

            await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using StreamReader sr = new(stream);
            json = await sr.ReadToEndAsync(cancellationToken);

            // Deserialize only once, this will throw an exception if the JSON is invalid
            T? data = JsonSerializer.Deserialize<T>(json, BuildInfo.JsonIndentOptionsWhenWritingNull);

            // Handle specific cases after deserialization, if needed
            if (typeof(T) == typeof(TokenResponse) && data is TokenResponse tokenResponse)
            {
                SetToken(tokenResponse);
            }

            // For special cases with replacement, handle JSON string manipulation
            if (typeof(T) == typeof(List<LineupPreviewChannel>) || typeof(T) == typeof(StationChannelMap))
            {
                json = json.Replace("[],", "");
                return JsonSerializer.Deserialize<T>(json);
            }

            if (typeof(T) == typeof(Dictionary<string, Dictionary<string, ScheduleMd5Response>>))
            {
                json = json.Replace("[]", "{}");
                return JsonSerializer.Deserialize<T>(json);
            }

            return data;
        }
        catch (JsonException ex)
        {
            logger.LogError("JSON deserialization error: {Message}, at byte position {BytePositionInLine}", ex.Message, ex.BytePositionInLine);

            if (json != null)
            {
                long bytePosition = ex.BytePositionInLine ?? 0;
                int start = Math.Max((int)bytePosition - 40, 0);
                string line = json.Substring(start, Math.Min(200, json.Length - start));

                if (!line.Contains("INVALID_PROGRAMID"))
                {
                    logger.LogError($"Invalid JSON near position: {line}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching the HTTP response.");
        }

        return default;
    }

    private void CreateHttpClient()
    {
        _httpClient = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        })
        {
            BaseAddress = new Uri(BaseAddress),
            Timeout = TimeSpan.FromSeconds(30)
        };
        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(settings.ClientUserAgent);
        _httpClient.DefaultRequestHeaders.ExpectContinue = true;
    }
}