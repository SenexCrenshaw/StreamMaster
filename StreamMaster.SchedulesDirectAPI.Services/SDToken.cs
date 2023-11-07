using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI.Services;

public class SDToken(ILogger<SDToken> logger, ISettingsService settingsService) : ISDToken
{
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    private readonly string _sdTokenFilename = Path.Combine(BuildInfo.SDCacheFolder, "sd_token.json");
    private string? _token;
    private DateTime _tokenDateTime;
    private DateTime _lockOutTokenDateTime;

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_token))
        {
            await LoadTokenAsync(cancellationToken);
        }

        if (!string.IsNullOrEmpty(_token) && _tokenDateTime.AddHours(23) > DateTime.UtcNow)
        {
            return _token;
        }

        if (_lockOutTokenDateTime > DateTime.UtcNow)
        {
            logger.LogWarning("Token retrieval is currently locked out.");
            return null;
        }

        _token = await RetrieveTokenAsync(cancellationToken).ConfigureAwait(false);

        return _token;
    }

    public async Task<string?> ResetTokenAsync(CancellationToken cancellationToken)
    {
        //_token = null;
        //await SaveTokenAsync(cancellationToken).ConfigureAwait(false);
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

    public async Task<string> GetAPIUrl(string command, CancellationToken cancellationToken)
    {
        string? token = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
        return ConstructAPIUrlWithToken(command, token);
    }

    private static string ConstructAPIUrlWithToken(string command, string? token)
    {
        return command.Contains('?') ? $"{SD_BASE_URL}{command}&token={token}" : $"{SD_BASE_URL}{command}?token={token}";
    }

    private async Task LoadTokenAsync(CancellationToken cancellationToken = default)
    {
        await _fileSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_sdTokenFilename))
            {
                _token = null;
                return;
            }

            string jsonString = File.ReadAllText(_sdTokenFilename);
            SDTokenFile? result = JsonSerializer.Deserialize<SDTokenFile>(jsonString)!;
            if (result is null)
            {
                _token = null;
                return;
            }

            _token = result.Token;
            _tokenDateTime = result.TokenDateTime;
            _lockOutTokenDateTime = result.LockOutTokenDateTime;
        }
        finally
        {
            _ = _fileSemaphore.Release();
        }
    }

    private async Task<string?> RetrieveTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            Setting setting = await settingsService.GetSettingsAsync(cancellationToken).ConfigureAwait(false);
            string? sdHashedPassword = HashHelper.TestSha1HexHash(setting.SDPassword) ? setting.SDPassword : setting.SDPassword.GetSHA1Hash();

            if (string.IsNullOrEmpty(sdHashedPassword))
            {
                return null;
            }

            SDGetTokenRequest data = new() { username = setting.SDUserName, password = sdHashedPassword };
            string jsonString = JsonSerializer.Serialize(data);
            using StringContent content = new(jsonString, Encoding.UTF8, "application/json");

            HttpClient httpClient = SDHelpers.CreateHttpClient("Mozilla/5.0 (compatible; streammaster/1.0)");

            using HttpResponseMessage response = await httpClient.PostAsync($"{SD_BASE_URL}token", content, cancellationToken).ConfigureAwait(false);
            (System.Net.HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, SDGetToken? result) = await SDHandler.ProcessResponse<SDGetToken?>(response, cancellationToken).ConfigureAwait(false);

            switch (responseCode)
            {
                case SDHttpResponseCode.OK:
                    if (!string.IsNullOrEmpty(result?.token))
                    {
                        logger.LogWarning("SD Retrieved Token");
                        _token = result.token;
                        _tokenDateTime = DateTime.UtcNow;
                        _lockOutTokenDateTime = DateTime.MinValue;
                        await SaveTokenAsync(cancellationToken);
                        return _token;
                    }
                    break;

                case SDHttpResponseCode.TOKEN_EXPIRED:
                case SDHttpResponseCode.TOKEN_MISSING:
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
            string jsonString = JsonSerializer.Serialize(tokenFile);

            await File.WriteAllTextAsync(_sdTokenFilename, jsonString, cancellationToken);
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