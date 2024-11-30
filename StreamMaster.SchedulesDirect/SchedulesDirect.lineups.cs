using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.Models;

namespace StreamMaster.SchedulesDirect;

public partial class SchedulesDirect
{
    public async Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken)
    {
        if (!_sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }

        List<CountryData>? countryData = await CountryDataCache.GetAsync<List<CountryData>>().ConfigureAwait(false);
        if (countryData != null)
        {
            return countryData;
        }

        Dictionary<string, List<Country>>? ret = await schedulesDirectAPI.GetApiResponse<Dictionary<string, List<Country>>>(APIMethod.GET, "available/countries", cancellationToken: cancellationToken);
        if (ret == null)
        {
            logger.LogError("Did not receive a response from Schedules Direct for a list of available countries.");
            return null;
        }

        logger.LogDebug("Successfully retrieved list of available countries from Schedules Direct.");
        List<CountryData> serializableDataList = ret
         .Select(kv => new CountryData { Key = kv.Key, Countries = kv.Value })
         .ToList();

        await CountryDataCache.SetAsync<List<CountryData>>(serializableDataList).ConfigureAwait(false);
        //await WriteToCacheAsync("AvailableCountries", serializableDataList, cancellationToken).ConfigureAwait(false);

        return serializableDataList;
    }

    public async Task<List<Headend>?> GetHeadendsByCountryPostal(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        if (!_sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }

        if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(postalCode))
        {
            logger.LogWarning("Country {country} or postal code {postalCode} is empty", country, postalCode);
            return null;
        }


        List<Headend>? headends = await HeadendCache.GetAsync<List<Headend>>($"{country}-{postalCode}");
        if (headends is not null)
        {
            return headends;
        }

        headends = await schedulesDirectAPI.GetApiResponse<List<Headend>>(APIMethod.GET, $"headends?country={country}&postalcode={postalCode}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (headends != null)
        {
            await HeadendCache.SetAsync<List<Headend>>($"{country}-{postalCode}", headends).ConfigureAwait(false);
            logger.LogDebug("Successfully retrieved the headends for {country} and postal code {postalCode}.", country, postalCode);
        }
        else
        {
            logger.LogError("Failed to get a response from Schedules Direct for the headends of {country} and postal code {postalCode}.", country, postalCode);
        }

        return headends;
    }

    public async Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup, CancellationToken cancellationToken)
    {
        if (!_sdSettings.CurrentValue.SDEnabled)
        {
            return null;
        }

        List<LineupPreviewChannel>? ret = await LineupPreviewChannelCache.GetAsync<List<LineupPreviewChannel>>(lineup).ConfigureAwait(false);

        if (ret != null)
        {
            return ret;
        }

        ret = await schedulesDirectAPI.GetApiResponse<List<LineupPreviewChannel>>(APIMethod.GET, $"lineups/preview/{lineup}", cancellationToken: cancellationToken);
        if (ret != null)
        {
            for (int i = 0; i < ret.Count; i++)
            {
                ret[i].Id = i;
            }

            //await WriteToCacheAsync("LineupPreviewChannel" + lineup, ret, cancellationToken).ConfigureAwait(false);
            await LineupPreviewChannelCache.SetAsync<List<LineupPreviewChannel>>(lineup, ret).ConfigureAwait(false);
            logger.LogDebug("Successfully retrieved the channels in lineup {lineup} for preview.", lineup);
        }
        else
        {
            logger.LogError("Did not receive a response from Schedules Direct for retrieval of lineup {lineup} for preview.", lineup);
        }

        return ret;
    }

    public async Task<int> AddLineup(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse<AddRemoveLineupResponse>(APIMethod.PUT, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug("Successfully added lineup {lineup} to account. serverID: {ret.ServerId} , message: {ret.Message} , changesRemaining: {ret.ChangesRemaining}", lineup, ret.ServerId, ret.Message, ret.ChangesRemaining);
            return ret.ChangesRemaining;
        }

        logger.LogError("Failed to get a response from Schedules Direct when trying to add lineup {lineup}.", lineup);
        return 0;
    }

    public async Task<bool> UpdateHeadEnd(string lineup, bool SubScribed, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse<AddRemoveLineupResponse>(APIMethod.PUT, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug("Successfully added lineup {lineup} to account. serverID: {ret.ServerId} , message: {ret.Message} , changesRemaining: {ret.ChangesRemaining}", lineup, ret.ServerId, ret.Message, ret.ChangesRemaining);
        }
        else
        {
            logger.LogError("Failed to get a response from Schedules Direct when trying to add lineup {lineup}.", lineup);
        }
        return ret != null;
    }

    public async Task<int> RemoveLineup(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse<AddRemoveLineupResponse>(APIMethod.DELETE, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug("Successfully removed lineup {lineup} from account. serverID: {ret.ServerId} , message: {ret.Message} , changesRemaining: {ret.ChangesRemaining}", lineup, ret.ServerId, ret.Message, ret.ChangesRemaining);
            JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);
            jobManager.SetForceNextRun(true);
            schedulesDirectDataService.SchedulesDirectData().RemoveLineup(lineup);
            return ret.ChangesRemaining;
        }
        else
        {
            logger.LogError("Failed to get a response from Schedules Direct when trying to remove lineup {lineup}.", lineup);
        }
        return -1;
    }
}