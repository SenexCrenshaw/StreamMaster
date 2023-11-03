using Microsoft.Extensions.Logging;

using StreamMaster.SchedulesDirectAPI.Helpers;

using StreamMasterDomain.Common;
using StreamMasterDomain.Services;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI.Services;

public class SDToken : ISDToken
{
    private (SDStatus? status, DateTime timestamp)? cacheEntry = null;
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    private  HttpClient _httpClient;
    private readonly string SD_TOKEN_FILENAME = Path.Combine(BuildInfo.AppDataFolder, "sd_token.json");
    private string? token;
    private string _clientUserAgent = string.Empty;
    private string _sdUserName = string.Empty;
    private string _sdPassword = string.Empty;
    private DateTime tokenDateTime;
    private DateTime lockOutTokenDateTime;

    private readonly ILogger<SDToken> _logger;
    private readonly ISettingsService _settingsService;
    public SDToken(ILogger<SDToken> logger, ISettingsService settingsService)
    {
        _logger = logger;
        _settingsService = settingsService;
        LoadToken();
    }

    private HttpClient httpClient
    {
        get
        {
            if (_httpClient == null)
            {
                Setting setting = _settingsService.GetSettingsAsync().Result;
                _httpClient = SDHelpers.CreateHttpClient(setting.ClientUserAgent);
            }
            return _httpClient;
        }
    }

    private string sdUserName
    {
        get
        {
            if (string.IsNullOrEmpty(_sdUserName))
            {
                Setting settings = _settingsService.GetSettingsAsync().Result;
                _sdUserName = settings.SDUserName;
            }
            return _sdUserName;
        }
    }

    private string sdPassword
    {
        get
        {
            if (string.IsNullOrEmpty(_sdPassword))
            {
                Setting settings = _settingsService.GetSettingsAsync().Result;
                _sdPassword = settings.SDPassword;
            }
            return _sdPassword;
        }
    }

    private string clientUserAgent
    {
        get
        {
            if (string.IsNullOrEmpty(_clientUserAgent))
            {
                Setting settings = _settingsService.GetSettingsAsync().Result;
                _clientUserAgent = settings.ClientUserAgent;
            }
            return _clientUserAgent;
        }
    }

    public async Task<string?> GetToken(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token) || tokenDateTime.AddHours(23) < DateTime.Now)
        {
            if (lockOutTokenDateTime >= DateTime.Now)
            {
                return null;
            }

            token = await RetrieveToken(cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
        }
        return token;
    }

    public async Task<string?> ResetToken(CancellationToken cancellationToken = default)
    {
        token = null;
        await SaveToken(cancellationToken);
        return await GetToken(cancellationToken);
    }

    public async Task LockOutToken(int minutes = 15, CancellationToken cancellationToken = default)
    {
        token = null;
        lockOutTokenDateTime = DateTime.Now.AddMinutes(minutes);
        await SaveToken(cancellationToken);
    }

    public async Task<string> GetAPIUrl(string command, CancellationToken cancellationToken)
    {
        string? token = await GetToken(cancellationToken).ConfigureAwait(false);
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



    private async Task LoadToken(CancellationToken cancellationToken = default)
    {
        await _fileSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(SD_TOKEN_FILENAME))
            {
                token = null;
                return;
            }

            string jsonString = File.ReadAllText(SD_TOKEN_FILENAME);
            SDTokenFile? result = JsonSerializer.Deserialize<SDTokenFile>(jsonString)!;
            if (result is null)
            {
                token = null;
                return;
            }

            token = result.Token;
            tokenDateTime = result.TokenDateTime;
            lockOutTokenDateTime = result.LockOutTokenDateTime;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }
    private async Task<string?> RetrieveToken(CancellationToken cancellationToken)
    {
        string? sdHashedPassword;
        if (HashHelper.TestSha1HexHash(sdPassword))
        {
            sdHashedPassword = sdPassword;
        }
        else
        {
            sdHashedPassword = sdPassword.GetSHA1Hash();
        }

        if (string.IsNullOrEmpty(sdHashedPassword))
        {
            return null;
        }

        SDGetTokenRequest data = new()
        {
            username = sdUserName,
            password = sdHashedPassword
        };

        string jsonString = JsonSerializer.Serialize(data);
        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await httpClient.PostAsync($"{SD_BASE_URL}token", content, cancellationToken).ConfigureAwait(false);
        try
        {
            (HttpStatusCode httpStatusCode, SDHttpResponseCode responseCode, string? responseContent, SDGetToken? result) = await SDHandler.ProcessResponse<SDGetToken?>(response, cancellationToken).ConfigureAwait(false);

            if (responseCode == SDHttpResponseCode.ACCOUNT_LOCKOUT || responseCode == SDHttpResponseCode.ACCOUNT_DISABLED || responseCode == SDHttpResponseCode.ACCOUNT_EXPIRED)
            {
                await LockOutToken(cancellationToken: cancellationToken);
                return null;
            }

            if (result == null || string.IsNullOrEmpty(result.token))
            {
                return null;
            }
            token = result.token;
            tokenDateTime = DateTime.Now;
            lockOutTokenDateTime = DateTime.MinValue;
            await SaveToken(cancellationToken);
            return token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex}");
            return null;
        }
    }

    private async Task SaveToken(CancellationToken cancellationToken = default)
    {
        await _fileSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (token is null)
            {
                return;
            }

            string jsonString = JsonSerializer.Serialize(new SDTokenFile { Token = token, TokenDateTime = tokenDateTime, LockOutTokenDateTime = lockOutTokenDateTime });
            lock (typeof(SDToken))
            {
                File.WriteAllText(SD_TOKEN_FILENAME, jsonString);
            }
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }
}
