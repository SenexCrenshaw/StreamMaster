using StreamMaster.SchedulesDirectAPI.Models;

using StreamMasterDomain.Common;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class SchedulesDirect
{
    private static HttpClient _httpClient;
    private readonly SDToken sdToken = null!;    // = "Mozilla/5.0 (compatible; streammaster/1.0)"
    public SchedulesDirect(string clientUserAgent, string sdUserName, string sdPassword)
    {
        _httpClient = CreateHttpClient(clientUserAgent);
        sdToken = new SDToken(clientUserAgent, sdUserName, sdPassword);
    }

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        string? url = await sdToken.GetAPIUrl("available/countries", cancellationToken);

        using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            Countries? result = JsonSerializer.Deserialize<Countries>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: ", ex);
            return null;
        }
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        string? url = await sdToken.GetAPIUrl($"headends?country={country}&postalcode={postalCode}", cancellationToken);

        using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            List<Headend>? result = JsonSerializer.Deserialize<List<Headend>>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: ", ex);
            return null;
        }
    }

    public async Task<bool> GetImageUrl(string source, string fileName, CancellationToken cancellationToken)
    {
        string? url = await sdToken.GetAPIUrl($"image/{source}", cancellationToken);
        if (url == null)
        {
            return false;
        }

        (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(url, fileName, cancellationToken).ConfigureAwait(false);

        return success;
    }

    public async Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken)
    {
        string? url = await sdToken.GetAPIUrl($"lineups/{lineUp}", cancellationToken).ConfigureAwait(false);

        using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            LineUpResult? result = JsonSerializer.Deserialize<LineUpResult>(responseString);
            if (result == null)
            {
                return null;
            }
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: ", ex);
            return null;
        }
    }

    public async Task<List<LineUpPreview>> GetLineUpPreviews(CancellationToken cancellationToken)
    {
        List<LineUpPreview> res = new();
        LineUpsResult? lineups = await GetLineups(cancellationToken);

        if (lineups is null)
        {
            return res;
        }

        foreach (Lineup lineup in lineups.Lineups)
        {
            string? url = await sdToken.GetAPIUrl($"lineups/preview/{lineup.LineupString}", cancellationToken);

            using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            try
            {
                _ = response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                List<LineUpPreview>? results = JsonSerializer.Deserialize<List<LineUpPreview>>(responseString);
                if (results == null)
                {
                    continue;
                }
                for (int index = 0; index < results.Count; index++)
                {
                    LineUpPreview? lineUpPreview = results[index];
                    lineUpPreview.LineUp = lineup.LineupString;
                    lineUpPreview.Id = index;
                }

                res.AddRange(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: ", ex);
                return null;
            }
        }

        return res;
    }

    public async Task<LineUpsResult?> GetLineups(CancellationToken cancellationToken)
    {
        string? url = await sdToken.GetAPIUrl("lineups", cancellationToken);

        //httpClient.DefaultRequestHeaders.Add("verboseMap", "true");

        using HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            LineUpsResult? result = JsonSerializer.Deserialize<LineUpsResult>(responseString);
            if (result == null)
            {
                return null;
            }

            return result;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<List<SDProgram>?> GetPrograms(List<string> programIds, CancellationToken cancellationToken)
    {
        //await CheckToken(cancellationToken).ConfigureAwait(false);

        //var dt = DateTime.Now;

        //List<string> toSend = new();
        //foreach (var programId in programIds)
        //{
        //    toSend.Add(new StationId(stationId));//, new List<string> { dt.ToShortDateString(), dt.AddDays(2).ToShortDateString(), }));
        //}

        string? url = await sdToken.GetAPIUrl("programs", cancellationToken);

        string jsonString = JsonSerializer.Serialize(programIds);
        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        using HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            List<SDProgram>? result = JsonSerializer.Deserialize<List<SDProgram>>(responseString);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: ", ex);
            return null;
        }
    }

    public async Task<List<Schedule>?> GetSchedules(List<string> stationIds, CancellationToken cancellationToken)
    {
        List<StationId> StationIds = new();
        foreach (string stationId in stationIds)
        {
            StationIds.Add(new StationId(stationId));
        }

        string? url = await sdToken.GetAPIUrl("schedules", cancellationToken);

        string jsonString = JsonSerializer.Serialize(StationIds);
        StringContent content = new(jsonString, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        try
        {
            _ = response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            List<Schedule>? result = JsonSerializer.Deserialize<List<Schedule>>(responseString);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: ", ex);
            return null;
        }
    }

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        List<Station>? stations = await GetStations(cancellationToken).ConfigureAwait(false);
        if (stations is null)
        {
            return new();
        }
        List<StationPreview> ret = new();
        for (int index = 0; index < stations.Count; index++)
        {
            Station? station = stations[index];
            StationPreview sp = new(station)
            {
                Id = index
            };
            ret.Add(sp);
        }
        return ret;
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        List<Station> ret = new();

        LineUpsResult? lineUps = await GetLineups(cancellationToken).ConfigureAwait(false);
        if (lineUps == null || lineUps.Lineups == null)
        {
            return ret;
        }

        foreach (Lineup lineUp in lineUps.Lineups)
        {
            LineUpResult? res = await GetLineup(lineUp.LineupString, cancellationToken).ConfigureAwait(false);
            if (res == null)
            {
                continue;
            }

            foreach (Station station in res.Stations)
            {
                station.LineUp = lineUp.LineupString;
            }
            ret.AddRange(res.Stations);
        }

        return ret;
    }

    public async Task<SDStatus?> GetStatus(CancellationToken cancellationToken)
    {
        return await sdToken.GetStatus(cancellationToken);
    }

    public async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        return await sdToken.GetSystemReady(cancellationToken);
    }

    private HttpClient CreateHttpClient(string clientUserAgent)
    {
        HttpClient client = new(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
        });
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd(clientUserAgent);

        return client;
    }
}
