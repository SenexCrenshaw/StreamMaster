using StreamMasterDomain.Common;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirect;

public class SchedulesDirect
{
    private const string SD_BASE_URL = "https://json.schedulesdirect.org/20141201/";
    private static readonly HttpClient httpClient = CreateHttpClient();

    public SchedulesDirect(string sdUserName, string sdPassword)
    {
        SDUserName = sdUserName;
        if (HashHelper.TestSha1HexHash(sdPassword))
        {
            SDHashedPassword = sdPassword;
        }
        else
        {
            SDHashedPassword = sdPassword.GetSHA1Hash();
        }
    }

    public string SDHashedPassword { get; private set; }

    public string SDUserName { get; private set; }

    public string Token { get; private set; }
    public DateTime TokenDateTime { get; private set; }

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        await CheckToken();

        var url = GetUrl("available/countries");

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<Countries>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        await CheckToken();

        var url = GetUrl($"headends?country={country}&postalcode={postalCode}");

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<List<Headend>>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken)
    {
        await CheckToken();

        var url = GetUrl($"lineups/{lineUp}");

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<LineUpResult>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<LineUpsResult?> GetLineups(CancellationToken cancellationToken)
    {
        await CheckToken();

        var url = GetUrl("lineups");

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<LineUpsResult>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<SDProgram>?> GetPrograms(List<string> programIds, CancellationToken cancellationToken)
    {
        await CheckToken();

        //var dt = DateTime.Now;

        //List<string> toSend = new();
        //foreach (var programId in programIds)
        //{
        //    toSend.Add(new StationId(stationId));//, new List<string> { dt.ToShortDateString(), dt.AddDays(2).ToShortDateString(), }));
        //}

        var url = GetUrl("programs");

        string jsonString = JsonSerializer.Serialize(programIds);
        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        using HttpResponseMessage response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<List<SDProgram>>(responseString);
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<Schedule>?> GetSchedules(List<string> stationIds, CancellationToken cancellationToken)
    {
        await CheckToken();

        //var dt = DateTime.Now;

        List<StationId> StationIds = new();
        foreach (var stationId in stationIds)
        {
            StationIds.Add(new StationId(stationId));//, new List<string> { dt.ToShortDateString(), dt.AddDays(2).ToShortDateString(), }));
        }

        var url = GetUrl("schedules");

        string jsonString = JsonSerializer.Serialize(StationIds);
        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<List<Schedule>>(responseString);
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<SDStatus?> GetStatus(CancellationToken cancellationToken)
    {
        await CheckToken();

        var url = GetUrl("status");

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<SDStatus>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            throw;
        }
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

    private async Task<bool> CheckToken(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(Token) || TokenDateTime.AddHours(23) < DateTime.Now)
        {
            await GetToken(cancellationToken);

            if (string.IsNullOrEmpty(Token))
                throw new ApplicationException("Unable to get token");

            TokenDateTime=  DateTime.Now;
        }
        return true;
    }

    private async Task<string?> GetToken(CancellationToken cancellationToken)
    {
        SDGetTokenRequest data = new()
        {
            username = SDUserName,
            password = SDHashedPassword
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
                return Token;
            }
            Token = result.token;
        }
        catch (Exception ex)
        {
            throw;
        }

        return Token;
    }

    private string GetUrl(string commnand)
    {
        if (commnand.Contains("?"))
        {
            return $"{SD_BASE_URL}{commnand}&token={Token}";
        }
        return $"{SD_BASE_URL}{commnand}?token={Token}";
    }
}

public class Oceania
{
    public string fullName { get; set; }
    public string postalCode { get; set; }
    public string postalCodeExample { get; set; }
    public string shortName { get; set; }
}
