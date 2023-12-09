using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain;
using StreamMaster.SchedulesDirectAPI.Domain.Enums;

using StreamMasterDomain.Common;

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public partial class SchedulesDirectAPI : ISchedulesDirectAPI
{
    private readonly ILogger<SchedulesDirectAPI> logger;
    private readonly IMemoryCache memoryCache;
    public HttpClient _httpClient = null!;
    private const string BaseAddress = "https://json.schedulesdirect.org/20141201/";

    public SchedulesDirectAPI(ILogger<SchedulesDirectAPI> logger, IMemoryCache memoryCache)
    {
        this.logger = logger;
        this.memoryCache = memoryCache;
        CreateHttpClient();
    }



    public async Task<HttpResponseMessage> GetSdImage(string uri)//, DateTimeOffset ifModifiedSince)
    {
        try
        {
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{BaseAddress}{uri}")
            };
            var response = await _httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            return !response.IsSuccessStatusCode || response.Content?.Headers?.ContentType?.MediaType == "application/json"
                ? response.Content?.Headers?.ContentType?.MediaType == "application/json"
                    ? await HandleHttpResponseError(response, await response.Content.ReadAsStringAsync())
                    : await HandleHttpResponseError(response, null)
                : response;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError($"{uri} GetSdImage() Exception: {ex?.InnerException.Message ?? ex.Message}");
        }
        return null;
    }

    private async Task<HttpResponseMessage> HandleHttpResponseError(HttpResponseMessage response, string? content)
    {
        var tokenUsed = response.RequestMessage.Headers.GetValues("token")?.FirstOrDefault();

        if (!string.IsNullOrEmpty(content))
        {
            var err = JsonSerializer.Deserialize<BaseResponse>(content);
            if (err != null)
            {
                var sdCode = (SDHttpResponseCode)err.Code;

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
                        response.StatusCode = (HttpStatusCode)423; // 423
                        response.ReasonPhrase = "Locked";
                        break;
                    case SDHttpResponseCode.IMAGE_NOT_FOUND: // IMAGE_NOT_FOUND
                    case SDHttpResponseCode.IMAGE_QUEUED: // IMAGE_QUEUED
                        response.StatusCode = HttpStatusCode.NotFound; // 404
                        response.ReasonPhrase = "Not Found";
                        break;
                    case SDHttpResponseCode.MAX_IMAGE_DOWNLOADS: // MAX_IMAGE_DOWNLOADS
                    case SDHttpResponseCode.MAX_IMAGE_DOWNLOADS_TRIAL: // MAX_IMAGE_DOWNLOADS_TRIAL
                        response.StatusCode = (HttpStatusCode)429; // 429
                        response.ReasonPhrase = "Too Many Requests";
                        break;
                    case SDHttpResponseCode.TOKEN_MISSING: // TOKEN_MISSING - special case when token is getting refreshed due to below responses from a separate request
                    case SDHttpResponseCode.INVALID_USER: // INVALID_USER
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
            logger.LogError($"{response.RequestMessage.RequestUri.AbsolutePath.Replace(BaseAddress, "/")}: {(int)response.StatusCode} {response.ReasonPhrase} : Token={tokenUsed[..5]}...{(!string.IsNullOrEmpty(content) ? $"\n{content}" : "")}");
        }

        return response;
    }

    /// <summary>
    /// 
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
            await tokenSemaphore.WaitAsync(cancellationToken);

            if (!GoodToken)
            {
                CheckToken();
                if (!GoodToken)
                {
                    return default;
                }
            }

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
            tokenSemaphore.Release();
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
                Content = (content != null)
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

            JsonSerializerOptions jsonOptions = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            if (typeof(T) != typeof(List<LineupPreviewChannel>) &&
                typeof(T) != typeof(StationChannelMap) &&
                typeof(T) != typeof(Dictionary<string, Dictionary<string, ScheduleMd5Response>>))
            {
                using var sr = new StreamReader(stream);
                json = await sr.ReadToEndAsync(cancellationToken);

                if (typeof(T) == typeof(TokenResponse))
                {
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json, jsonOptions);
                    if (tokenResponse != null)
                    {
                        SetToken(tokenResponse);
                    }
                }
                var data = JsonSerializer.Deserialize<T>(json, jsonOptions);
                return data;
            }

            if (typeof(T) == typeof(List<LineupPreviewChannel>) || typeof(T) == typeof(StationChannelMap))
            {
                using var sr = new StreamReader(stream);
                json = await sr.ReadToEndAsync(cancellationToken);
                json = json.Replace("[],", ""); // Modify the JSON string as needed
                return JsonSerializer.Deserialize<T>(json);
            }

            if (typeof(T) == typeof(Dictionary<string, Dictionary<string, ScheduleMd5Response>>))
            {
                using var sr = new StreamReader(stream);
                json = await sr.ReadToEndAsync(cancellationToken);
                json = json.Replace("[]", "{}"); // Modify the JSON string as needed
                return JsonSerializer.Deserialize<T>(json);
            }

            using var jsonStream = new StreamReader(stream);
            return await JsonSerializer.DeserializeAsync<T>(jsonStream.BaseStream, cancellationToken: cancellationToken);
        }
        catch (JsonException ex)
        {
            // Handle the JSON deserialization error
            // Log the error and decide how to proceed

            if (json != null)
            {
                var bytePosition = ex.BytePositionInLine ?? 0;

                var startOriginal = Math.Max((int)bytePosition, 0);
                var start = Math.Max((int)bytePosition - 40, 0);

                var lineOriginal = json.Substring(startOriginal, 200);
                var line = json.Substring(start, 200);
                if (line.Contains("INVALID_PROGRAMID"))
                {


                }
                else
                {
                    Debug.Assert(true);
                }
            }

        }
        catch (Exception)
        {
            Debug.Assert(true);
        }
        return default;
    }




    private void CreateHttpClient()
    {
        var setting = memoryCache.GetSetting();
        _httpClient = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        })
        {
            BaseAddress = new Uri(BaseAddress),
            Timeout = TimeSpan.FromMinutes(5)
        };
        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(setting.ClientUserAgent);
        _httpClient.DefaultRequestHeaders.ExpectContinue = true;
    }

}
