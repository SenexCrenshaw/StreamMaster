using StreamMaster.SchedulesDirectAPI.Models;

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
}

public class SDToken
{
    private (SDStatus? status, DateTime timestamp)? cacheEntry = null;
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    private readonly HttpClient httpClient = CreateHttpClient();
    private readonly string SD_TOKEN_FILENAME = Path.Combine(BuildInfo.AppDataFolder, "sd_token.json");
    private string? token;
    private static string _clientUserAgent = "Mozilla/5.0 (compatible; streammaster/1.0)";
    private readonly string _sdUserName;
    private readonly string _sdPassword;
    private DateTime tokenDateTime;

    public SDToken(string clientUserAgent, string sdUserName, string sdPassword)
    {
        _sdUserName = sdUserName;
        _sdPassword = sdPassword;
        _clientUserAgent = clientUserAgent;
        LoadToken();
    }

    public async Task<SDStatus?> GetStatus(CancellationToken cancellationToken)
    {
        (SDStatus? status, bool resetToken) = await GetStatusInternal(cancellationToken);
        if (resetToken)
        {
            if (await ResetToken(cancellationToken).ConfigureAwait(false) == null)
            {
                return GetSDStatusOffline();
            }
            return status;
        }

        return status;
    }

    public async Task<(SDStatus? sdStatus, bool resetToken)> GetStatusInternal(CancellationToken cancellationToken)
    {
        if (cacheEntry.HasValue && (DateTime.UtcNow - cacheEntry.Value.timestamp).TotalMinutes < 10)
        {
            return (cacheEntry.Value.status, false);
        }

        string url = await GetAPIUrl("status", cancellationToken);

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.NotFound)
            {
                return (null, true);
            }

            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (responseString == null)
            {
                return (null, false);
            }
            SDStatus? result = JsonSerializer.Deserialize<SDStatus>(responseString);
            if (result == null)
            {
                return (null, false);
            }
            cacheEntry = (result, DateTime.UtcNow);
            return (result, false);
        }
        catch (Exception)
        {
            return (null, false);
        }
    }

    public async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        (SDStatus? status, bool resetToken) = await GetStatusInternal(cancellationToken);
        if (resetToken && await ResetToken(cancellationToken).ConfigureAwait(false) != null)
        {
            (status, _) = await GetStatusInternal(cancellationToken);
        }

        return status?.systemStatus[0].status?.ToLower() == "online";
    }

    public async Task<string?> GetToken(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token) || tokenDateTime.AddHours(23) < DateTime.Now)
        {
            token = await RetrieveToken(cancellationToken);

            if (string.IsNullOrEmpty(token))
            {
                throw new ApplicationException("Unable to get token");
            }

            tokenDateTime = DateTime.Now;
            SaveToken();
        }
        return token;
    }

    public async Task<string?> ResetToken(CancellationToken cancellationToken = default)
    {
        token = null;

        return await GetToken(cancellationToken);
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

    private static SDStatus GetSDStatusOffline()
    {
        SDStatus ret = new();
        ret.systemStatus.Add(new SDSystemstatus { status = "Offline" });
        return ret;
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
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            SDGetToken? result = JsonSerializer.Deserialize<SDGetToken>(responseString);
            if (result == null || string.IsNullOrEmpty(result.token))
            {
                return null;
            }
            token = result.token;
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

        string jsonString = JsonSerializer.Serialize(new SDTokenFile { Token = token, TokenDateTime = tokenDateTime });
        lock (typeof(SDToken))
        {
            File.WriteAllText(SD_TOKEN_FILENAME, jsonString);
        }
    }
}
