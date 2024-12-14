using System.Collections.Concurrent;

using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.SchedulesDirect.Services;

public interface ISchedulesDirectRepository
{
    Task<UserStatus?> GetUserStatusAsync(CancellationToken cancellationToken);

    Task<List<CountryData>?> GetAvailableCountriesAsync(CancellationToken cancellationToken);

    Task<List<Headend>?> GetHeadendsByCountryPostalAsync(string country, string postalCode, CancellationToken cancellationToken);

    Task<List<LineupPreviewChannel>?> GetLineupPreviewChannelAsync(string lineup, CancellationToken cancellationToken);

    Task<int> AddLineupAsync(string lineup, CancellationToken cancellationToken);

    Task<int> RemoveLineupAsync(string lineup, CancellationToken cancellationToken);

    Task<bool> UpdateHeadEndAsync(string lineup, bool subscribed, CancellationToken cancellationToken);

    Task<Dictionary<string, GenericDescription>?> GetDescriptionsAsync(string[] seriesIds, CancellationToken cancellationToken);

    Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] requests, CancellationToken cancellationToken);

    Task<List<Programme>?> GetProgramsAsync(string[] programIds, CancellationToken cancellationToken);

    Task<LineupResponse?> GetSubscribedLineupsAsync(CancellationToken cancellationToken);

    Task<LineupResult?> GetLineupResultAsync(string lineup, CancellationToken cancellationToken);

    Task<HttpResponseMessage?> GetSdImageAsync(string uri, CancellationToken cancellationToken);

    Task<List<ProgramMetadata>?> GetArtworkAsync(string[] programIds, CancellationToken cancellationToken);

    Task DownloadImageResponsesAsync(List<string> imageQueue, ConcurrentBag<ProgramMetadata> metadata, int start, CancellationToken cancellationToken);
}