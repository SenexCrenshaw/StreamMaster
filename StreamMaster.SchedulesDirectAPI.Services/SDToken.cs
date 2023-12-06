using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Domain;
using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;
using StreamMaster.SchedulesDirectAPI.Domain.Models;
using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using System.Net;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI.Services;

public class SDToken(ILogger<SDToken> logger, ISettingsService settingsService, IMemoryCache memoryCache) : ISDToken
{
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    //private readonly string _sdTokenFilename = Path.Combine(BuildInfo.SDCacheFolder, "sd_token.json");
    private string? _token;
    private DateTime _tokenDateTime;
    private DateTime _lockOutTokenDateTime;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

       public async Task<string?> GetTokenAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Schedules Direct Get Token");
        if (string.IsNullOrEmpty(_token))
        {
            await LoadTokenAsync(cancellationToken);
        }

        if (!string.IsNullOrEmpty(_token) && _tokenDateTime.AddHours(1) > DateTime.UtcNow)
        {
            return _token;
        }

        if (_lockOutTokenDateTime > DateTime.UtcNow)
        {
            logger.LogWarning("Token retrieval is currently locked out.");
            return null;
        }

        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check the token after acquiring the lock
            if (!string.IsNullOrEmpty(_token) && _tokenDateTime.AddHours(1) > DateTime.UtcNow)
            {
                return _token;
            }

            _token = await RetrieveTokenAsync(cancellationToken).ConfigureAwait(false);
            if (! string.IsNullOrEmpty(_token) )
            {
                logger.LogInformation("Schedules Direct Retrieved Token Successful");
                return _token;
                
            }
            else
            {
                logger.LogError("Schedules Direct Retrieved Token Error");
                return null;
            }
           
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    public async Task<string?> ResetTokenAsync(CancellationToken cancellationToken)
    {
        //_token = null;
        await SaveTokenAsync(cancellationToken).ConfigureAwait(false);
        _token = await RetrieveTokenAsync(cancellationToken).ConfigureAwait(false);
        return _token;
        //return await GetTokenAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task LockOutTokenAsync(int minutes = 15, CancellationToken cancellationToken = default)
    {
        _token = null;
        _lockOutTokenDateTime = DateTime.UtcNow.AddMinutes(minutes);
        await SaveTokenAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<string?> GetAPIUrl(string command, CancellationToken cancellationToken)
    {
        string? token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
        if ( string.IsNullOrEmpty(token) )
        {
            return null;
        }
        return ConstructAPIUrlWithToken(command, token);
    }

    private static string ConstructAPIUrlWithToken(string command, string? token)
    {
        return command.Contains('?') ? $"{SD_BASE_URL}{command}&token={token}" : $"{SD_BASE_URL}{command}?token={token}";
    }

    private async Task LoadTokenAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Schedules Direct Loading Token");

        await _fileSemaphore.WaitAsync(cancellationToken);
        try
        {
            //if (!File.Exists(_sdTokenFilename))
            //{
            //    _token = null;
            //    return;
            //}

            //string jsonString = File.ReadAllText(_sdTokenFilename);
            //SDTokenFile? result = JsonSerializer.Deserialize<SDTokenFile>(jsonString)!;
            //if (result is null)
            //{
            //    _token = null;
            //    return;
            //}

            var result = memoryCache.GetSDToken();
            if (result != null && ! string.IsNullOrEmpty(result.Token))
            {
                _token = result.Token;
                _tokenDateTime = result.TokenDateTime;
                _lockOutTokenDateTime = result.LockOutTokenDateTime;
                logger.LogDebug("Schedules Direct Loading Token Successful");
            }
            else
            {
                logger.LogDebug("Schedules Direct Loading Token, none found");
            }
        }
        finally
        {
            _ = _fileSemaphore.Release();
        }
    }

    private static async Task<(HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, T? data)> ProcessResponse<T>(HttpResponseMessage response, ILogger logger, CancellationToken cancellationToken)
    {
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        SDHttpResponseCode responseCode = SDHttpResponseCode.UNKNOWN_ERROR;

        try
        {
            TokenResponse? responseObj = JsonSerializer.Deserialize<TokenResponse>(responseContent);
            if (responseObj is not null)
            {
                responseCode = (SDHttpResponseCode)responseObj.Code;
                if (responseCode == SDHttpResponseCode.OK)
                {
                    T? result = JsonSerializer.Deserialize<T>(responseContent);
                    if (result != null)
                    {
                        return (response.StatusCode, responseCode, responseContent, result);
                    }
                }
            }                       
        }
        catch (JsonException ex)
        {
            logger.LogWarning("Deserialization to type {Type} failed: {Message}", typeof(T).Name, ex.Message);
        }
        return (response.StatusCode, responseCode, responseContent, default(T?));
    }
    private async Task<string?> RetrieveTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            Setting setting = memoryCache.GetSetting();
            string? sdHashedPassword = HashHelper.TestSha1HexHash(setting.SDSettings.SDPassword) ? setting.SDSettings.SDPassword : setting.SDSettings.SDPassword.GetSHA1Hash();

            if (string.IsNullOrEmpty(sdHashedPassword))
            {
                return null;
            }

            SDGetTokenRequest data = new() { username = setting.SDSettings.SDUserName, password = sdHashedPassword };
            string jsonString = JsonSerializer.Serialize(data);
            using StringContent content = new(jsonString, Encoding.UTF8, "application/json");

            HttpClient httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);

            using HttpResponseMessage response = await httpClient.PostAsync($"{SD_BASE_URL}token", content, cancellationToken).ConfigureAwait(false);
            (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, SDGetToken? result) = await ProcessResponse<SDGetToken?>(response, logger, cancellationToken).ConfigureAwait(false);
           
            if (responseCode != SDHttpResponseCode.OK) { 
            logger.LogWarning("Schedules Direct Retrieve Token Response Code: {responseCode}", responseCode.GetMessage());
        }
            switch (responseCode)
            {
                case SDHttpResponseCode.OK:
                    if (!string.IsNullOrEmpty(result?.token))
                    {
                        logger.LogInformation("SD Retrieved Token");
                        _token = result.token;
                        _tokenDateTime = DateTime.UtcNow;
                        _lockOutTokenDateTime = DateTime.MinValue;
                        await SaveTokenAsync(cancellationToken);
                        return _token;
                    }
                    break;

                case SDHttpResponseCode.TOKEN_EXPIRED:
                case SDHttpResponseCode.TOKEN_MISSING:
                case SDHttpResponseCode.INVALID_USER:
                    // Retrieve a new token if possible
                    logger.LogError("SD Token missing!");
                    // Your logic to retrieve a new token goes here
                    break;

                case SDHttpResponseCode.ACCOUNT_LOCKOUT:
                case SDHttpResponseCode.ACCOUNT_DISABLED:
                case SDHttpResponseCode.ACCOUNT_EXPIRED:
                    // Log the issue and possibly inform the user
                    string message = responseCode.GetMessage();
                    logger.LogWarning("SD Account issue: {message}", message);
                    await LockOutTokenAsync(cancellationToken: cancellationToken);
                    break;

                default:
                    // Log unexpected status codes
                    logger.LogError("SD Unexpected response code: {responseCode}", responseCode.GetMessage());
                    break;
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SD An error occurred while retrieving the token");
            return null;
        }
    }

    private async Task SaveTokenAsync(CancellationToken cancellationToken = default)
    {
        await _fileSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_token is null)
            {
                return;
            }

            SDTokenFile tokenFile = new() { Token = _token, TokenDateTime = _tokenDateTime, LockOutTokenDateTime = _lockOutTokenDateTime };
            memoryCache.SetSDToken(tokenFile);
            //string jsonString = JsonSerializer.Serialize(tokenFile);

            //await File.WriteAllTextAsync(_sdTokenFilename, jsonString, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while saving the token to the file");
        }
        finally
        {
            _ = _fileSemaphore.Release();
        }
    }
}