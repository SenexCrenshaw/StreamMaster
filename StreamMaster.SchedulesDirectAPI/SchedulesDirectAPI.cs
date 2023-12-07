using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain;
using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;
using StreamMasterDomain.Models;
using StreamMasterDomain.Services;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StreamMaster.SchedulesDirectAPI;

public class SchedulesDirectAPI(ILogger<SchedulesDirectAPI> logger, ISDToken SdToken, ISettingsService settingsService) : ISchedulesDirectAPI
{
    public static readonly int MAX_RETRIES = 2;
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private readonly SemaphoreSlim _cacheSemaphore = new(1, 1);

    private async Task WriteToCacheAsync<T>(string name, T data, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);
            SDCacheEntry<T> cacheEntry = new()
            {
                Data = data,
                Command = name,
                Content = "",
                Timestamp = DateTime.UtcNow
            };

            string contentToCache = JsonSerializer.Serialize(cacheEntry);
            await File.WriteAllTextAsync(cachePath, contentToCache, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
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
            logger.LogDebug($"HTTP request exception thrown. Message:{FileUtil.ReportExceptionMessages(ex)}");
        }
        return default;
    }


    private async Task<T?> GetHttpResponse<T>(HttpMethod method, string command, object? content = null, CancellationToken cancellationToken = default)
    {
        string? url = await SdToken.GetAPIUrl(command, cancellationToken);

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);
        string? json = null;
        HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);

        int retryCount = 0;
        while (retryCount < 3)
        {
            retryCount++;
            try
            {
                using HttpRequestMessage request = new(method, url)
                {
                    Content = (content != null)
                        ? new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
                        : null
                };


                using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                //using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    if (await HandleHttpResponseError(response, await response.Content?.ReadAsStringAsync(), cancellationToken))
                    {
                        return default;
                    }
                    await Task.Delay(100, cancellationToken);
                    continue;
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
                    return JsonSerializer.Deserialize<T>(json, jsonOptions);
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
                    var startOrig = Math.Max((int)ex.BytePositionInLine, 0);
                    var start = Math.Max((int)ex.BytePositionInLine - 40, 0);
                    var lineOrig = json.Substring(startOrig, 200);
                    var line = json.Substring(start, 200);
                    if (line.Contains("INVALID_PROGRAMID"))
                    {


                    }
                    else
                    {
                        Debug.Assert(true);
                        var a = 1;
                    }
                }

            }
            catch (Exception)
            {
                Debug.Assert(true);
            }

        }
        return default;
    }

    private async Task<bool> HandleHttpResponseError(HttpResponseMessage response, string? content, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(content) || !(response.Content?.Headers?.ContentType?.MediaType?.Contains("json") ?? false))
        {
            logger.LogDebug($"HTTP request failed with status code \"{(int)response.StatusCode} {response.ReasonPhrase}\"");
        }
        else
        {
            var err = JsonSerializer.Deserialize<BaseResponse>(content);
            logger.LogDebug($"SD responded with error code: {err.Code} , message: {err.Message ?? err.Response} , serverID: {err.ServerId} , datetime: {err.Datetime:s}Z");
            //SdErrorMessage = $"{err.Response}: {err.Message}";

            var responseCode = (SDHttpResponseCode)err.Code;

            if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED or SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
            {
                return await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null && false;
            }

            logger.LogDebug("SD Error: response code: {responseCode}", responseCode.GetMessage());

        }
        return false;
    }
}
