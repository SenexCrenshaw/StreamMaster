using StreamMaster.SchedulesDirectAPI.Domain.Models;
using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISchedulesDirect
{
    Task<List<SubscribedLineup>> GetLineups(CancellationToken cancellationToken);
    List<StationChannelName> GetStationChannelNames();
    XMLTV? CreateXmltv(string baseUrl, IEnumerable<string>? stationIds = null);
    Task<StationChannelMap?> GetStationChannelMap(string lineup);
    Task<bool> AddLineup(string lineup, CancellationToken cancellationToken);
    Task<List<ProgramMetadata>?> GetArtworkAsync(string[] request);
    Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken);
    Task<List<string>?> GetCustomLogosFromServerAsync(string server);
    Task<Dictionary<string, GenericDescription>?> GetGenericDescriptionsAsync(string[] request);
    Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default);
    //Task<bool> GetImageUrl(string programId, ProgramArtwork icon, CancellationToken cancellationToken);
    Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup, CancellationToken cancellationToken);
    Task<List<Programme>?> GetProgramsAsync(string[] request);
    Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] request);
    Task<Dictionary<string, Dictionary<string, ScheduleMd5Response>>?> GetScheduleMd5sAsync(ScheduleRequest[] request);
    Task<StationChannelMap?> GetStationChannelMapAsync(string lineup);
    Task<UserStatus> GetStatus(CancellationToken cancellationToken);
    Task<LineupResponse?> GetSubscribedLineups(CancellationToken cancellationToken);
    Task<bool> GetSystemReady(CancellationToken cancellationToken);
    Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken);
    void ResetCache(string command);

    Task<bool> SDSync(CancellationToken cancellationToken);
    Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    Task<List<StationChannelMap>> GetStationChannelMaps(CancellationToken cancellationToken);

    //Task DownloadStationLogos(CancellationToken cancellationToken);
}