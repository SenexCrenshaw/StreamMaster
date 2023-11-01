
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using System.Net;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI.Services;

public class SDTokenService(IHttpClientFactory httpClientFactory, ISDTokenCache tokenCache, ISDTokenFileHandler tokenFileHandler, ISettingsService settingsService, ILogger logger) : ISDTokenService
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient();
    public static readonly int MAX_RETRIES = 1;
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    private SDToken sdToken;

    private async Task<bool> EnsureTokenAsync(CancellationToken cancellationToken = default)
    {
        if (sdToken != null)
        {
            return true;
        }

        Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
        if (!setting.SDEnabled)
        {
            return false;
        }

        sdToken = new SDToken(setting.ClientUserAgent, setting.SDUserName, setting.SDPassword);
        return true;
    }

    public async Task<string> GetAPIUrl(string command, CancellationToken cancellationToken)
    {
        string? token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
        return ConstructAPIUrlWithToken(command, token);
    }

    private static string ConstructAPIUrlWithToken(string command, string? token)
    {
        if (command.Contains('?'))
        {
            return $"{SD_BASE_URL}{command}&token={token}";
        }
        return $"{SD_BASE_URL}{command}?token={token}";
    }

    public async Task<ISDStatus?> GetStatusAsync(CancellationToken cancellationToken)
    {
        //try
        //{
        //    SDStatus? cachedStatus = tokenCache.GetStatus();
        //    if (cachedStatus != null)
        //    {
        //        return cachedStatus;
        //    }

        //    SDStatus? status = await GetStatusInternalAsync(cancellationToken).ConfigureAwait(false);
        //    if (status != null)
        //    {
        //        tokenCache.SetStatus(status);
        //    }
        //    return status;
        //}
        //catch (Exception ex)
        //{
        //    logger.LogWarning("Failed to get status. Error: {Error}", ex.Message);
        //    return null;
        //}
        return null;
    }


    private async Task<ISDStatus?> GetStatusInternalAsync(CancellationToken cancellationToken)
    {
        int retry = 0;
        while (retry <= MAX_RETRIES)
        {
            try
            {
                string url = await GetAPIUrl("status", cancellationToken).ConfigureAwait(false);
                using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, SDStatus? result) = await SDHandler.ProcessResponse<SDStatus?>(response, cancellationToken).ConfigureAwait(false);

                switch (responseCode)
                {
                    case SDHttpResponseCode.ACCOUNT_LOCKOUT:
                    case SDHttpResponseCode.ACCOUNT_DISABLED:
                    case SDHttpResponseCode.ACCOUNT_EXPIRED:
                        LockOutToken(15);
                        logger.LogWarning("Account is locked out, disabled, or expired. Locking out for 15 minutes.");
                        return GetSDStatusOffline();

                    case SDHttpResponseCode.TOKEN_EXPIRED:
                    case SDHttpResponseCode.INVALID_USER:
                        logger.LogWarning("Token expired or invalid user. Resetting token.");
                        if (await ResetTokenAsync(cancellationToken).ConfigureAwait(false) == null)
                        {
                            return GetSDStatusOffline();
                        }
                        break;

                    default:
                        if (result != null)
                        {
                            return result;
                        }
                        logger.LogWarning("Received null result from API. Retrying.");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("An error occurred while getting status. Error: {Error}", ex.Message);
                return GetSDStatusOffline();
            }
            retry++;
        }
        logger.LogWarning("Max retries reached. Returning offline status.");
        return GetSDStatusOffline();
    }



    public async Task<string?> ResetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!await EnsureTokenAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        return await sdToken.ResetToken(cancellationToken).ConfigureAwait(false);
    }

    private static SDStatus GetSDStatusOffline()
    {
        SDStatus ret = new();
        ret.systemStatus.Add(new SDSystemStatus { status = "Offline" });
        return ret;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            string? token = tokenFileHandler.LoadToken();
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }

            token = await RetrieveTokenAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(token))
            {
                tokenFileHandler.SaveToken(token);
            }
            return token;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to get token. Error: {Error}", ex.Message);
            return null;
        }
    }

    private async Task<string?> RetrieveTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            Setting setting = await settingsService.GetSettingsAsync();
            if (!setting.SDEnabled)
            {
                return null;
            }

            string? sdHashedPassword;
            if (HashHelper.TestSha1HexHash(setting.SDPassword))
            {
                sdHashedPassword = setting.SDPassword;
            }
            else
            {
                sdHashedPassword = setting.SDPassword.GetSHA1Hash();
            }

            if (string.IsNullOrEmpty(sdHashedPassword))
            {
                logger.LogWarning("Hashed password is null or empty.");
                return null;
            }

            SDGetTokenRequest data = new()
            {
                username = setting.SDUserName,
                password = sdHashedPassword
            };

            string jsonString = JsonSerializer.Serialize(data);
            StringContent content = new(jsonString, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync($"{SD_BASE_URL}token", content, cancellationToken).ConfigureAwait(false);

            (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, SDGetToken? result) = await SDHandler.ProcessResponse<SDGetToken?>(response, cancellationToken).ConfigureAwait(false);

            switch (responseCode)
            {
                case SDHttpResponseCode.ACCOUNT_LOCKOUT:
                case SDHttpResponseCode.ACCOUNT_DISABLED:
                case SDHttpResponseCode.ACCOUNT_EXPIRED:
                    LockOutToken(15);
                    logger.LogWarning("Account is locked out, disabled, or expired. Locking out for 15 minutes.");
                    return null;

                default:
                    if (result != null && !string.IsNullOrEmpty(result.token))
                    {
                        logger.LogInformation("Successfully retrieved token.");
                        return result.token;
                    }
                    logger.LogWarning("Failed to retrieve token. Response code: {ResponseCode}", responseCode);
                    return null;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning("An error occurred while retrieving token. Error: {Error}", ex.Message);
            return null;
        }
    }

    public async Task LockOutToken(int minutes = 15)
    {
        if (!await EnsureTokenAsync().ConfigureAwait(false))
        {
            return;
        }

        sdToken.LockOutToken(minutes);
    }

}
