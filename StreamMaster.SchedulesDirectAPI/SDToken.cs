using StreamMasterDomain.Common;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class SDTokenFile
{
    public string? Token { get; set; }
    public DateTime TokenDateTime { get; set; }
    public DateTime LockOutTokenDateTime { get; set; }
}

public class SDToken
{
    private static (SDStatus? status, DateTime timestamp)? cacheEntry = null;
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    public static readonly int MAX_RETRIES = 1;
    private readonly HttpClient httpClient = CreateHttpClient();
    private readonly string SD_TOKEN_FILENAME = Path.Combine(BuildInfo.AppDataFolder, "sd_token.json");
    private static string? token;
    private static string _clientUserAgent = "Mozilla/5.0 (compatible; streammaster/1.0)";
    private readonly string _sdUserName;
    private readonly string _sdPassword;
    private static DateTime tokenDateTime;
    private static DateTime lockOutTokenDateTime;
    /// <summary>
    /// Initializes a new instance of the <see cref="SDToken"/> class.
    /// </summary>
    /// <param name="clientUserAgent">The user agent for the client.</param>
    /// <param name="sdUserName">The username for the SD service.</param>
    /// <param name="sdPassword">The password for the SD service.</param>
    /// <remarks>
    /// This constructor initializes the SDToken object and calls the LoadToken method to populate the token.
    /// </remarks>
    public SDToken(string clientUserAgent, string sdUserName, string sdPassword)
    {
        _sdUserName = sdUserName ?? throw new ArgumentNullException(nameof(sdUserName));
        _sdPassword = sdPassword ?? throw new ArgumentNullException(nameof(sdPassword));
        _clientUserAgent = clientUserAgent ?? throw new ArgumentNullException(nameof(clientUserAgent));
        LoadToken();
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
        SaveToken();
        return await GetToken(cancellationToken);
    }

    public void LockOutToken(int minutes = 15)
    {
        token = null;
        lockOutTokenDateTime = DateTime.Now.AddMinutes(minutes);
        SaveToken();
    }

    private static HttpClient CreateHttpClient()
    {

        HttpClient client = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(_clientUserAgent);

        return client;
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



    private void LoadToken()
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
    private async Task<string?> RetrieveToken(CancellationToken cancellationToken)
    {
        string? sdHashedPassword;
        if (HashHelper.TestSha1HexHash(_sdPassword))
        {
            sdHashedPassword = _sdPassword;
        }
        else
        {
            sdHashedPassword = _sdPassword.GetSHA1Hash();
        }

        if (string.IsNullOrEmpty(sdHashedPassword))
        {
            return null;
        }

        SDGetTokenRequest data = new()
        {
            username = _sdUserName,
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
                lockOutTokenDateTime = DateTime.Now.AddMinutes(15);
                SaveToken();
                return null;
            }

            if (result == null || string.IsNullOrEmpty(result.token))
            {
                return null;
            }
            token = result.token;
            tokenDateTime = DateTime.Now;
            lockOutTokenDateTime = DateTime.MinValue;
            SaveToken();
            return token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex}");
            return null;
        }
    }

    private void SaveToken()
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
}
