using System.Collections.Concurrent;

using StreamMaster.SchedulesDirect.Domain.Models;


namespace StreamMaster.SchedulesDirect.Services;

public class SchedulesDirectAPIService(ISchedulesDirectRepository schedulesDirectRepository, IHttpService httpService) : ISchedulesDirectAPIService
{

    // Metadata-related methods
    public async Task<List<CountryData>?> GetAvailableCountriesAsync(CancellationToken cancellationToken)
    {
        return !await httpService.ValidateTokenAsync(cancellationToken: cancellationToken)
            ? null
            : await schedulesDirectRepository.GetAvailableCountriesAsync(cancellationToken);
    }

    public async Task<List<Headend>?> GetHeadendsByCountryPostalAsync(string country, string postalCode, CancellationToken cancellationToken)
    {
        return !await ValidateTokenAsync(cancellationToken: cancellationToken)
            ? null
            : await schedulesDirectRepository.GetHeadendsByCountryPostalAsync(country, postalCode, cancellationToken);
    }

    public async Task<List<LineupPreviewChannel>?> GetLineupPreviewChannelAsync(string lineup, CancellationToken cancellationToken)
    {
        return !await ValidateTokenAsync(cancellationToken: cancellationToken)
            ? null
            : await schedulesDirectRepository.GetLineupPreviewChannelAsync(lineup, cancellationToken);
    }

    public async Task<int> AddLineupAsync(string lineup, CancellationToken cancellationToken)
    {
        return !await httpService.ValidateTokenAsync(cancellationToken: cancellationToken)
            ? 0
            : await schedulesDirectRepository.AddLineupAsync(lineup, cancellationToken);
    }

    public async Task<int> RemoveLineupAsync(string lineup, CancellationToken cancellationToken)
    {
        return !await httpService.ValidateTokenAsync(cancellationToken: cancellationToken)
            ? -1
            : await schedulesDirectRepository.RemoveLineupAsync(lineup, cancellationToken);
    }

    public async Task<bool> UpdateHeadEndAsync(string lineup, bool subscribed, CancellationToken cancellationToken)
    {
        return await httpService.ValidateTokenAsync(cancellationToken: cancellationToken)
&& await schedulesDirectRepository.UpdateHeadEndAsync(lineup, subscribed, cancellationToken);
    }
    public Task<Dictionary<string, GenericDescription>?> GetDescriptionsAsync(string[] seriesIds, CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.GetDescriptionsAsync(seriesIds, cancellationToken);
    }

    public Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] requests, CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.GetScheduleListingsAsync(requests, cancellationToken);
    }

    public Task<List<Programme>?> GetProgramsAsync(string[] programIds, CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.GetProgramsAsync(programIds, cancellationToken);
    }

    public Task<LineupResponse?> GetSubscribedLineupsAsync(CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.GetSubscribedLineupsAsync(cancellationToken);
    }

    public Task<LineupResult?> GetLineupResultAsync(string lineup, CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.GetLineupResultAsync(lineup, cancellationToken);
    }

    public Task<HttpResponseMessage?> GetSdImageAsync(string uri, CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.GetSdImageAsync(uri, cancellationToken);
    }

    public Task<List<ProgramMetadata>?> GetArtworkAsync(string[] programIds, CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.GetArtworkAsync(programIds, cancellationToken);
    }

    public Task DownloadImageResponsesAsync(List<string> imageQueue, ConcurrentBag<ProgramMetadata> metadata, int start, CancellationToken cancellationToken)
    {
        return schedulesDirectRepository.DownloadImageResponsesAsync(imageQueue, metadata, start, cancellationToken);
    }

    // Token-related methods
    public Task RefreshTokenAsync(CancellationToken cancellationToken)
    {
        return httpService.RefreshTokenAsync(cancellationToken);
    }

    public Task<bool> ValidateTokenAsync(bool forceReset = false, CancellationToken cancellationToken = default)
    {
        return httpService.ValidateTokenAsync(forceReset, cancellationToken);
    }

    public void ClearToken()
    {
        httpService.ClearToken();
    }
}
