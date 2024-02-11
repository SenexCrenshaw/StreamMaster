using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Models;
using StreamMaster.SchedulesDirect.Domain.Enums;
using StreamMaster.SchedulesDirect.Helpers;


namespace StreamMaster.SchedulesDirect;
public partial class SchedulesDirect
{

    public async Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken)
    {
        List<CountryData>? cache = await GetValidCachedDataAsync<List<CountryData>>("AvailableCountries", cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            return cache;
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

        await WriteToCacheAsync("AvailableCountries", serializableDataList, cancellationToken).ConfigureAwait(false);

        return serializableDataList;
    }

    public async Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(country) || string.IsNullOrEmpty(postalCode))
        {
            logger.LogWarning($"Country {country} or postal code {postalCode} is empty");
            return null;
        }

        List<Headend>? cache = await GetValidCachedDataAsync<List<Headend>>($"Headends-{country}-{postalCode}", cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            return cache;
        }

        cache = await schedulesDirectAPI.GetApiResponse<List<Headend>>(APIMethod.GET, $"headends?country={country}&postalcode={postalCode}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (cache != null)
        {
            await WriteToCacheAsync($"Headends-{country}-{postalCode}", cache, cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Successfully retrieved the headends for {country} and postal code {postalCode}.");
        }
        else
        {
            logger.LogError($"Failed to get a response from Schedules Direct for the headends of {country} and postal code {postalCode}.");
        }

        return cache;
    }

    public async Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup, CancellationToken cancellationToken)
    {
        List<LineupPreviewChannel>? ret = await GetValidCachedDataAsync<List<LineupPreviewChannel>>("LineupPreviewChannel" + lineup, cancellationToken).ConfigureAwait(false);

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

            await WriteToCacheAsync("LineupPreviewChannel" + lineup, ret, cancellationToken).ConfigureAwait(false);
            logger.LogDebug($"Successfully retrieved the channels in lineup {lineup} for preview.");
        }
        else
        {
            logger.LogError($"Did not receive a response from Schedules Direct for retrieval of lineup {lineup} for preview.");
        }

        return ret;
    }

    public async Task<bool> AddLineup(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse<AddRemoveLineupResponse>(APIMethod.PUT, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug($"Successfully added lineup {lineup} to account. serverID: {ret.ServerId} , message: {ret.Message} , changesRemaining: {ret.ChangesRemaining}");
        }
        else
        {
            logger.LogError($"Failed to get a response from Schedules Direct when trying to add lineup {lineup}.");
        }
        return ret != null;
    }

    public async Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken)
    {
        AddRemoveLineupResponse? ret = await schedulesDirectAPI.GetApiResponse<AddRemoveLineupResponse>(APIMethod.DELETE, $"lineups/{lineup}", cancellationToken: cancellationToken).ConfigureAwait(false);
        if (ret != null)
        {
            logger.LogDebug($"Successfully removed lineup {lineup} from account. serverID: {ret.ServerId} , message: {ret.Message} , changesRemaining: {ret.ChangesRemaining}");
            JobStatusManager jobManager = jobStatusService.GetJobManager(JobType.SDSync, EPGHelper.SchedulesDirectId);
            jobManager.SetForceNextRun(true);
        }
        else
        {
            logger.LogError("Failed to get a response from Schedules Direct when trying to remove lineup {lineup}.", lineup);
        }
        return ret != null;
    }
}