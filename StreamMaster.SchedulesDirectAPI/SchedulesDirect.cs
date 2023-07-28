using StreamMasterDomain.Common;

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamMaster.SchedulesDirectAPI;

public class StationPreview
{
    public StationPreview()
    { }

    public StationPreview(Station station)
    {
        Affiliate = station.Affiliate;
        Callsign = station.Callsign;
        LineUp = station.LineUp;
        Name = station.Name;
        StationId = station.StationID;
    }

    public string Affiliate { get; set; }
    public string Callsign { get; set; }
    public int Id { get; internal set; }
    public string LineUp { get; set; }
    public string Name { get; set; }
    public string StationId { get; set; }
}

public class SchedulesDirect
{
    private static readonly HttpClient httpClient = CreateHttpClient();

    public SchedulesDirect()
    {
    }

    public async Task<Countries?> GetCountries(CancellationToken cancellationToken)
    {
        var url = await SDToken.GetAPIUrl("available/countries", cancellationToken);

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
        var url = await SDToken.GetAPIUrl($"headends?country={country}&postalcode={postalCode}", cancellationToken);

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

    public async Task<bool> GetImageUrl(string source, string fileName, CancellationToken cancellationToken)
    {
        var url = await SDToken.GetAPIUrl($"image/{source}", cancellationToken);
        if (url == null)
        {
            return false;
        }

        (bool success, Exception? ex) = await FileUtil.DownloadUrlAsync(url, fileName, cancellationToken).ConfigureAwait(false);

        return success;
    }

    public async Task<LineUpResult?> GetLineup(string lineUp, CancellationToken cancellationToken)
    {
        var url = await SDToken.GetAPIUrl($"lineups/{lineUp}", cancellationToken).ConfigureAwait(false);

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

    public async Task<List<LineUpPreview>> GetLineUpPreviews(CancellationToken cancellationToken)
    {
        var res = new List<LineUpPreview>();
        var lineups = await GetLineups(cancellationToken);

        if (lineups is null)
        {
            return res;
        }

        foreach (var lineup in lineups.Lineups)
        {
            var url = await SDToken.GetAPIUrl($"lineups/preview/{lineup.LineupString}", cancellationToken);

            using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            try
            {
                _ = response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var results = JsonSerializer.Deserialize<List<LineUpPreview>>(responseString);
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
                continue;
            }
        }

        return res;
    }

    public async Task<LineUpsResult?> GetLineups(CancellationToken cancellationToken)
    {
        var url = await SDToken.GetAPIUrl("lineups", cancellationToken);

        //httpClient.DefaultRequestHeaders.Add("verboseMap", "true");

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
        //await CheckToken(cancellationToken).ConfigureAwait(false);

        //var dt = DateTime.Now;

        //List<string> toSend = new();
        //foreach (var programId in programIds)
        //{
        //    toSend.Add(new StationId(stationId));//, new List<string> { dt.ToShortDateString(), dt.AddDays(2).ToShortDateString(), }));
        //}

        var url = await SDToken.GetAPIUrl("programs", cancellationToken);

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
        List<StationId> StationIds = new();
        foreach (var stationId in stationIds)
        {
            StationIds.Add(new StationId(stationId));
        }

        var url = await SDToken.GetAPIUrl("schedules", cancellationToken);

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

    public async Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken)
    {
        var stations = await GetStations(cancellationToken).ConfigureAwait(false);
        if (stations is null)
        {
            return new();
        }
        var ret = new List<StationPreview>();
        for (int index = 0; index < stations.Count; index++)
        {
            Station? station = stations[index];
            var sp = new StationPreview(station)
            {
                Id = index
            };
            ret.Add(sp);
        }
        return ret;
    }

    public async Task<List<Station>> GetStations(CancellationToken cancellationToken)
    {
        var ret = new List<Station>();

        var lineUps = await GetLineups(cancellationToken).ConfigureAwait(false);
        if (lineUps == null || lineUps.Lineups == null)
        {
            return ret;
        }

        foreach (var lineUp in lineUps.Lineups)
        {
            var res = await GetLineup(lineUp.LineupString, cancellationToken).ConfigureAwait(false);
            if (res == null)
                continue;

            foreach (var station in res.Stations)
            {
                station.LineUp = lineUp.LineupString;
            }
            ret.AddRange(res.Stations);
        }

        return ret;
    }

    public async Task<SDStatus?> GetStatus(CancellationToken cancellationToken)
    {
        return await SDToken.GetStatus(cancellationToken);
    }

    public async Task<bool> GetSystemReady(CancellationToken cancellationToken)
    {
        return await SDToken.GetSystemReady(cancellationToken);
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
}
