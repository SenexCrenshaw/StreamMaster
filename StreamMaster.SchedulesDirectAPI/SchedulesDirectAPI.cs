using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain;
using StreamMaster.SchedulesDirectAPI.Domain.Enums;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;
using StreamMasterDomain.Models;
using StreamMasterDomain.Services;

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


    public static string ReportExceptionMessages(Exception ex)
    {
        var ret = string.Empty;
        var innerException = ex;
        do
        {
            ret += $" {innerException.Message} ";
            innerException = innerException.InnerException;
        } while (innerException != null);
        return ret;
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
                    return await GetHttpResponse<T>(HttpMethod.Get, uri,cancellationToken: cancellationToken);
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
            logger.LogDebug($"HTTP request exception thrown. Message:{ReportExceptionMessages(ex)}");
        }
        return default;
    }


    private async Task<T?> GetHttpResponse<T>(HttpMethod method, string command, object? content = null, CancellationToken cancellationToken = default)
    {
        string? url = await SdToken.GetAPIUrl(command, cancellationToken);

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

        using HttpRequestMessage request = new(method, url)
        {
            Content = (content != null)
                ? new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json")
                : null
        };

        HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);

        using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            return await HandleHttpResponseError<T>(response, await response.Content?.ReadAsStringAsync());
        }

        JsonSerializerOptions jsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        if (typeof(T) != typeof(List<LineupPreviewChannel>) &&
            typeof(T) != typeof(StationChannelMap) &&
            typeof(T) != typeof(Dictionary<string, Dictionary<string, ScheduleMd5Response>>))
        {
            return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), jsonOptions);
        }

        using Stream stream = await response.Content.ReadAsStreamAsync();

        if (typeof(T) == typeof(List<LineupPreviewChannel>) || typeof(T) == typeof(StationChannelMap))
        {
            using var sr = new StreamReader(stream);
            var json = await sr.ReadToEndAsync();
            json = json.Replace("[],", ""); // Modify the JSON string as needed
            return JsonSerializer.Deserialize<T>(json);
        }

        if (typeof(T) == typeof(Dictionary<string, Dictionary<string, ScheduleMd5Response>>))
        {
            using var sr = new StreamReader(stream);
            var json = await sr.ReadToEndAsync();
            json = json.Replace("[]", "{}"); // Modify the JSON string as needed
            return JsonSerializer.Deserialize<T>(json);
        }

        using var jsonStream = new StreamReader(stream);
        return await JsonSerializer.DeserializeAsync<T>(jsonStream.BaseStream);
    }

    private async Task<T> HandleHttpResponseError<T>(HttpResponseMessage response, string content)
    {
        if (string.IsNullOrEmpty(content) || !(response.Content?.Headers?.ContentType?.MediaType?.Contains("json") ?? false)) logger.LogDebug($"HTTP request failed with status code \"{(int)response.StatusCode} {response.ReasonPhrase}\"");
        else
        {
            var err = JsonSerializer.Deserialize<BaseResponse>(content);
            logger.LogDebug($"SD responded with error code: {err.Code} , message: {err.Message ?? err.Response} , serverID: {err.ServerId} , datetime: {err.Datetime:s}Z");
            //SdErrorMessage = $"{err.Response}: {err.Message}";

            var responseCode = (SDHttpResponseCode)err.Code;
            logger.LogDebug("SD Error: response code: {responseCode}", responseCode.GetMessage());

        }
        return default;
    }

    //public async Task<T?> GetData<T>(HttpMethod method, string command, object? content = null, CancellationToken cancellationToken = default, bool dontCache = false)
    //{
    //    string cacheKey = SDHelpers.GenerateCacheKey(command);
    //    string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);

    //    if (!dontCache && method == HttpMethod.Get)
    //    {
    //        // Check if cache exists and is valid
    //        TimeSpan duration = CacheDuration;
    //        if (command == SDCommands.Status || command == SDCommands.LineUps)
    //        {
    //            duration = TimeSpan.FromMinutes(5);
    //        }

    //        string? token = await EnsureToken(cancellationToken);
    //        if (!string.IsNullOrEmpty(token) && File.Exists(cachePath) && DateTime.UtcNow - File.GetLastWriteTimeUtc(cachePath) <= duration)
    //        {
    //            string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken);
    //            SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);

    //            if (cacheEntry != null && DateTime.UtcNow - cacheEntry.Timestamp <= duration)
    //            {
    //                return cacheEntry.Data;
    //            }
    //        }
    //    }

    //    try
    //    {

    //        string? url = await SdToken.GetAPIUrl(command, cancellationToken);

    //        Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

    //        HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
    //        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            return HandleHttpResponseError<T>(response, await response.Content?.ReadAsStringAsync());
    //        }

    //        (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, logger, cancellationToken).ConfigureAwait(false);

    //        if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
    //        {
    //            await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
    //            return default;
    //        }

    //        if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
    //        {
    //            if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
    //            {
    //                return default;
    //            }
    //            ++retry;
    //            continue;
    //        }

    //        if (result == null)
    //        {
    //            return default;
    //        }

    //        if (!dontCache)
    //        {
    //            SDCacheEntry<T> entry = new()
    //            {
    //                Timestamp = DateTime.UtcNow,
    //                Command = command,
    //                Content = "",
    //                Data = result
    //            };
    //            string jsonResult = JsonSerializer.Serialize(entry);
    //            await File.WriteAllTextAsync(cachePath, jsonResult, cancellationToken);
    //        }
    //        return result;

    //    }
    //    catch (Exception)
    //    {
    //        return default;
    //    }
    //    return default;
    //}

    private async Task<T?> GetValidCachedDataAsync<T>(string name, CancellationToken cancellationToken = default)
    {
        await _cacheSemaphore.WaitAsync(cancellationToken);
        try
        {
            string cacheKey = SDHelpers.GenerateCacheKey(name);
            string cachePath = Path.Combine(BuildInfo.SDCacheFolder, cacheKey);
            if (!File.Exists(cachePath))
            {
                return default;
            }

            string cachedContent = await File.ReadAllTextAsync(cachePath, cancellationToken).ConfigureAwait(false);
            SDCacheEntry<T>? cacheEntry = JsonSerializer.Deserialize<SDCacheEntry<T>>(cachedContent);

            return cacheEntry != null && DateTime.Now - cacheEntry.Timestamp <= CacheDuration ? cacheEntry.Data : default;
        }
        finally
        {
            _ = _cacheSemaphore.Release();
        }
    }

    //public async Task<T?> PostData<T>(string command, object toPost, CancellationToken cancellationToken)
    //{
    //    string jsonString = JsonSerializer.Serialize(toPost);

    //    StringContent content = new(jsonString, Encoding.UTF8, "application/json");
    //    string? responseContent = "";
    //    int retry = 0;
    //    try
    //    {
    //        while (retry <= MAX_RETRIES)
    //        {
    //            ++retry;
    //            string? url = await SdToken.GetAPIUrl(command, cancellationToken);
    //            Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

    //            HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
    //            using HttpResponseMessage response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

    //            (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, logger, cancellationToken).ConfigureAwait(false);

    //            if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
    //            {
    //                await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
    //                return default;
    //            }

    //            if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
    //            {
    //                if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
    //                {
    //                    return default;
    //                }

    //                continue;
    //            }

    //            return result == null ? default : result;
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        Console.WriteLine($"Exception cannot deserialize data for {command} to {typeof(T).Name}");
    //        Console.WriteLine(responseContent);
    //        return default;
    //    }
    //    return default;
    //}

    //public async Task<T?> DeleteData<T>(string command, CancellationToken cancellationToken)
    //{
    //    int retry = 0;
    //    try
    //    {
    //        while (retry <= MAX_RETRIES)
    //        {
    //            string? url = await SdToken.GetAPIUrl(command, cancellationToken);

    //            Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

    //            HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
    //            using HttpResponseMessage response = await httpClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);

    //            (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, logger, cancellationToken).ConfigureAwait(false);

    //            if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
    //            {
    //                await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
    //                return default;
    //            }

    //            if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
    //            {
    //                if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
    //                {
    //                    return default;
    //                }
    //                ++retry;
    //                continue;
    //            }

    //            return result ?? default;
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        return default;
    //    }
    //    return default;
    //}

    //public async Task<T?> PutData<T>(string command, CancellationToken cancellationToken = default)
    //{
    //    int retry = 0;
    //    try
    //    {

    //        while (retry <= MAX_RETRIES)
    //        {
    //            string? url = await SdToken.GetAPIUrl(command, cancellationToken);

    //            Setting setting = await settingsService.GetSettingsAsync(cancellationToken);

    //            HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
    //            using HttpResponseMessage response = await httpClient.PutAsync(url, null, cancellationToken).ConfigureAwait(false);

    //            (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? result) = await SDHandler.ProcessResponse<T?>(response, logger, cancellationToken).ConfigureAwait(false);

    //            if (responseCode is SDHttpResponseCode.ACCOUNT_LOCKOUT or SDHttpResponseCode.ACCOUNT_DISABLED or SDHttpResponseCode.ACCOUNT_EXPIRED)
    //            {
    //                await SdToken.LockOutTokenAsync(cancellationToken: cancellationToken);
    //                return default;
    //            }

    //            if (responseCode is SDHttpResponseCode.TOKEN_EXPIRED or SDHttpResponseCode.INVALID_USER)
    //            {
    //                if (await SdToken.ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
    //                {
    //                    return default;
    //                }
    //                ++retry;
    //                continue;
    //            }

    //            return result ?? default;
    //        }
    //    }
    //    catch (Exception)
    //    {
    //        return default;
    //    }
    //    return default;
    //}

}
