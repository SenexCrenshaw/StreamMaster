using System.Collections.Concurrent;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;
using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.SchedulesDirect.Services;

public class SchedulesDirectRepository(
        ILogger<SchedulesDirectRepository> logger,
        IHttpService httpService,
        SMCacheManager<CountryData> CountryCache,
        SMCacheManager<Headend> HeadendCache,
        SMCacheManager<LineupPreviewChannel> LineupPreviewChannelCache,
        IOptionsMonitor<SDSettings> sdSettings
    ) : ISchedulesDirectRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(23);

    public async Task<Dictionary<string, GenericDescription>?> GetDescriptionsAsync(string[] seriesIds, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }

        if (seriesIds == null || seriesIds.Length == 0)
        {
            logger.LogWarning("No series IDs provided.");
            return null;
        }

        return await httpService.SendRequestAsync<Dictionary<string, GenericDescription>?>(
            APIMethod.POST,
            "metadata/description/",
            seriesIds,
            cancellationToken
        );
    }

    public async Task<UserStatus?> GetUserStatusAsync(CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }
        try
        {
            // Fetch user status from the API
            UserStatus? userStatus = await httpService.SendRequestAsync<UserStatus>(
                APIMethod.GET,
                "status",
                null,
                cancellationToken
            );

            if (userStatus != null)
            {
                // Log account details
                logger.LogInformation(
                    "Account expires: {AccountExpires:s}Z , Lineups: {LineupCount}/{MaxLineups} , Last update: {LastDataUpdate:s}Z",
                    userStatus.Account.Expires.ToString("s"),
                    userStatus.Lineups.Count,
                    userStatus.Account.MaxLineups,
                    userStatus.LastDataUpdate.ToString("s")
                );

                // Warn if the account is about to expire
                TimeSpan timeToExpire = userStatus.Account.Expires - SMDT.UtcNow;
                if (timeToExpire < TimeSpan.FromDays(7.0))
                {
                    logger.LogWarning("Your Schedules Direct account expires in {Days} days.", timeToExpire.Days.ToString("D2"));
                }

                return userStatus;
            }

            // Log error if no response
            logger.LogError("Did not receive a response from Schedules Direct for the status request.");
            throw new InvalidOperationException("Schedules Direct API returned null for status.");
        }
        catch (Exception ex)
        {
            // Handle exceptions explicitly
            logger.LogError(ex, "Error occurred while fetching user status.");
            throw;
        }
    }

    public async Task<List<CountryData>?> GetAvailableCountriesAsync(CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }
        List<CountryData>? cachedData = await CountryCache.GetAsync<List<CountryData>>().ConfigureAwait(false);
        if (cachedData != null)
        {
            logger.LogDebug("Retrieved available countries from cache.");
            return cachedData;
        }

        // Fetch data from the API if not in cache
        Dictionary<string, List<Country>>? response = await httpService.SendRequestAsync<Dictionary<string, List<Country>>>(
            APIMethod.GET,
            "available/countries",
            null,
            cancellationToken
        );

        if (response == null)
        {
            logger.LogError("Failed to retrieve available countries.");
            return null;
        }

        // Transform and cache the data
        List<CountryData> countryDataList = response
            .Select(kv => new CountryData { Key = kv.Key, Countries = kv.Value })
            .ToList();

        await CountryCache.SetAsync<List<CountryData>>(countryDataList).ConfigureAwait(false);
        logger.LogDebug("Cached available countries.");

        return countryDataList;
    }

    public async Task<List<Headend>?> GetHeadendsByCountryPostalAsync(string country, string postalCode, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }
        if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(postalCode))
        {
            logger.LogWarning("Country or postal code is empty.");
            return null;
        }

        // Generate cache key
        string cacheKey = $"{country}-{postalCode}";

        // Attempt to retrieve from cache
        List<Headend>? cachedData = await HeadendCache.GetAsync<List<Headend>>(cacheKey).ConfigureAwait(false);
        if (cachedData != null)
        {
            logger.LogDebug("Retrieved headends for {Country} and {PostalCode} from cache.", country, postalCode);
            return cachedData;
        }

        // Fetch data from API if not in cache
        List<Headend>? headends = await httpService.SendRequestAsync<List<Headend>>(
            APIMethod.GET,
            $"headends?country={country}&postalcode={postalCode}",
            null,
            cancellationToken
        );

        if (headends != null)
        {
            // Cache the data
            await HeadendCache.SetAsync(cacheKey, headends).ConfigureAwait(false);
            logger.LogDebug("Successfully retrieved and cached headends for {Country} and {PostalCode}.", country, postalCode);
        }
        else
        {
            logger.LogError("Failed to retrieve headends for {Country} and {PostalCode} from Schedules Direct.", country, postalCode);
        }

        return headends;
    }

    public async Task<List<LineupPreviewChannel>?> GetLineupPreviewChannelAsync(string lineup, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }
        if (string.IsNullOrEmpty(lineup))
        {
            logger.LogWarning("Lineup parameter is empty.");
            return null;
        }

        // Attempt to retrieve from cache
        List<LineupPreviewChannel>? cachedData = await LineupPreviewChannelCache.GetAsync<List<LineupPreviewChannel>>(lineup).ConfigureAwait(false);
        if (cachedData != null)
        {
            logger.LogDebug("Retrieved lineup preview for {Lineup} from cache.", lineup);
            return cachedData;
        }

        // Fetch data from API if not in cache
        List<LineupPreviewChannel>? previewChannels = await httpService.SendRequestAsync<List<LineupPreviewChannel>>(
            APIMethod.GET,
            $"lineups/preview/{lineup}",
            null,
            cancellationToken
        );

        if (previewChannels != null)
        {
            // Assign unique IDs to each channel
            for (int i = 0; i < previewChannels.Count; i++)
            {
                previewChannels[i].Id = i;
            }

            // Cache the retrieved data
            await LineupPreviewChannelCache.SetAsync(lineup, previewChannels).ConfigureAwait(false);
            logger.LogDebug("Successfully retrieved and cached lineup preview for {Lineup}.", lineup);
        }
        else
        {
            logger.LogError("Failed to retrieve lineup preview for {Lineup} from Schedules Direct.", lineup);
        }

        return previewChannels;
    }

    public async Task<int> AddLineupAsync(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? response = await httpService.SendRequestAsync<AddRemoveLineupResponse>(
            APIMethod.PUT,
            $"lineups/{lineup}",
            null,
            cancellationToken
        );

        return response?.ChangesRemaining ?? 0;
    }

    public async Task<int> RemoveLineupAsync(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? response = await httpService.SendRequestAsync<AddRemoveLineupResponse>(
            APIMethod.DELETE,
            $"lineups/{lineup}",
            null,
            cancellationToken
        );

        return response?.ChangesRemaining ?? -1;
    }

    public async Task<bool> UpdateHeadEndAsync(string lineup, bool subscribed, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? response = await httpService.SendRequestAsync<AddRemoveLineupResponse>(
            APIMethod.PUT,
            $"lineups/{lineup}",
            null,
            cancellationToken
        );

        return response != null;
    }

    public async Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] requests, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }
        if (requests == null || requests.Length == 0)
        {
            logger.LogWarning("No schedule requests provided.");
            return null;
        }

        return await httpService.SendRequestAsync<List<ScheduleResponse>?>(
            APIMethod.POST,
            "schedules",
            requests,
            cancellationToken
        );
    }

    public async Task<List<Programme>?> GetProgramsAsync(string[] programIds, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }
        if (programIds == null || programIds.Length == 0)
        {
            logger.LogWarning("No program IDs provided.");
            return null;
        }

        return await httpService.SendRequestAsync<List<Programme>?>(
            APIMethod.POST,
            "programs",
            programIds,
            cancellationToken
        );
    }

    public async Task<LineupResponse?> GetSubscribedLineupsAsync(CancellationToken cancellationToken)
    {
        return !sdSettings.CurrentValue.SDEnabled
            ? null
            : await httpService.SendRequestAsync<LineupResponse?>(
            APIMethod.GET,
            "lineups",
            null,
            cancellationToken
        );
    }

    public async Task<LineupResult?> GetLineupResultAsync(string lineup, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }

        if (string.IsNullOrEmpty(lineup))
        {
            logger.LogWarning("No lineup provided.");
            return null;
        }

        return await httpService.SendRequestAsync<LineupResult?>(
            APIMethod.GET,
            $"lineups/{lineup}",
            null,
            cancellationToken
        );
    }

    public async Task<HttpResponseMessage?> GetSdImageAsync(string uri, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }

        try
        {
            HttpRequestMessage request = new(HttpMethod.Get, uri);
            HttpResponseMessage response = await httpService.SendRawRequestAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode || response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                logger.LogWarning("Invalid response for GetSdImage request to {Uri}", uri);
                return null;
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching image for {Uri}", uri);
            return null;
        }
    }

    public async Task<List<ProgramMetadata>?> GetArtworkAsync(string[] programIds, CancellationToken cancellationToken)
    {
        if (!sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }

        if (programIds == null || programIds.Length == 0)
        {
            logger.LogWarning("No program IDs provided for artwork retrieval.");
            return null;
        }

        const int maxIdLength = 10;
        programIds = programIds.Select(id => id.StartsWith("MV") ? id : id.Truncate(maxIdLength)).ToArray();
        //string? a = programIds.FirstOrDefault(a => a.StartsWith("MV00001843"));
        //if (a is not null)
        //{
        //    int aaa = 1;
        //}
        return await httpService.SendRequestAsync<List<ProgramMetadata>?>(
            APIMethod.POST,
            "metadata/programs/",
            programIds,
            cancellationToken
        );
    }

    public async Task DownloadImageResponsesAsync(List<string> imageQueue, ConcurrentBag<ProgramMetadata> metadata, int start, CancellationToken cancellationToken)
    {
        if (imageQueue.Count - start < 1)
        {
            logger.LogWarning("No items in image queue to process.");
            return;
        }

        string[] series = new string[Math.Min(imageQueue.Count - start, SDAPIConfig.MaxImgQueries)];
        for (int i = 0; i < series.Length; ++i)
        {
            series[i] = imageQueue[start + i];
        }

        ProgramMetadata? a = metadata.FirstOrDefault(a => a.ProgramId.Contains("4571616"));

        List<ProgramMetadata>? responses = await GetArtworkAsync(series, cancellationToken);
        if (responses != null)
        {
            foreach (ProgramMetadata response in responses)
            {
                metadata.Add(response);
            }
        }
        else
        {
            logger.LogInformation(
                "No response received for artwork info of {Count} programs. First entry: {FirstEntry}",
                series.Length,
                series.Length > 0 ? series[0] : "N/A"
            );
        }
    }
}