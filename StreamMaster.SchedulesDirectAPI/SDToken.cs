using StreamMasterDomain.Common;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class SDTokenFile
{
    public string Token { get; set; }
    public DateTime TokenDateTime { get; set; }
}

public static class SDToken
{
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    private static readonly HttpClient httpClient = CreateHttpClient();
    private static readonly string SD_TOKEN_FILENAME = Path.Combine(BuildInfo.AppDataFolder, "sd_token.json");
    private static string? token;
    private static DateTime tokenDateTime;

    static SDToken()
    {
        LoadToken();
    }

    public static async Task<string?> GetAPIUrl(string commnand, CancellationToken cancellationToken)
    {
        var isReady = await GetSystemReady(cancellationToken).ConfigureAwait(false);
        if (!isReady)
        {
            return null;
        }

        return await GetAPIUrlInternal(commnand, cancellationToken);
    }

    public static async Task<SDStatus?> GetStatus(CancellationToken cancellationToken)
    {
        var (status, resetToken) = await GetStatusInternal(cancellationToken);
        if (resetToken)
        {
            if (await ResetToken().ConfigureAwait(false) == null)
            {
                return GetSDStatusOffline();
            }
            (status, _) = await GetStatusInternal(cancellationToken);
        }

        return status;
    }

    public static async Task<(SDStatus? sdStatus, bool resetToken)> GetStatusInternal(CancellationToken cancellationToken)
    {
        var url = await GetAPIUrlInternal("status", cancellationToken);

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return (null, true);
            }

            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (responseString == null)
            {
                return (null, false);
            }
            var result = JsonSerializer.Deserialize<SDStatus>(responseString);
            if (result == null)
            {
                return (null, false);
            }
            return (result, false);
        }
        catch (Exception ex)
        {
            return (null, false);
        }
    }

    public static async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        var (status, resetToken) = await GetStatusInternal(cancellationToken);
        if (resetToken)
        {
            if (await ResetToken().ConfigureAwait(false) == null)
            {
                return false;
            }
            (status, _) = await GetStatusInternal(cancellationToken);
            if (status.systemStatus[0].status.ToLower() != "online")
            {
                return false;
            }
        }

        return true;
    }

    public static async Task<string?> GetToken(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token) || tokenDateTime.AddHours(23) < DateTime.Now)
        {
            token = await RetrieveToken(cancellationToken);

            if (string.IsNullOrEmpty(token))
                throw new ApplicationException("Unable to get token");

            tokenDateTime = DateTime.Now;
            SaveToken();
        }
        return token;
    }

    public static async Task<string?> ResetToken(CancellationToken cancellationToken = default)
    {
        token = null;

        return await GetToken(cancellationToken);
    }

    private static HttpClient CreateHttpClient()
    {
        var setting = FileUtil.GetSetting();
        var client = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(setting.ClientUserAgent);

        return client;
    }

    private static async Task<string> GetAPIUrlInternal(string commnand, CancellationToken cancellationToken)
    {
        var token = await GetToken(cancellationToken).ConfigureAwait(false);

        if (commnand.Contains("?"))
        {
            return $"{SD_BASE_URL}{commnand}&token={token}";
        }
        return $"{SD_BASE_URL}{commnand}?token={token}";
    }

    private static SDStatus GetSDStatusOffline()
    {
        var ret = new SDStatus();
        ret.systemStatus.Add(new SDSystemstatus { status = "Offline" });
        return ret;
    }

    private static void LoadToken()
    {
        if (!File.Exists(SD_TOKEN_FILENAME))
        {
            token = null;
            return;
        }

        string jsonString = File.ReadAllText(SD_TOKEN_FILENAME);
        var result = JsonSerializer.Deserialize<SDTokenFile>(jsonString)!;
        if (result is null)
        {
            token = null;
            return;
        }

        token = result.Token;
        tokenDateTime = result.TokenDateTime;
    }

    private static async Task<string?> RetrieveToken(CancellationToken cancellationToken)
    {
        var setting = FileUtil.GetSetting();

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
            throw;
        }
    }

    private static void SaveToken()
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
