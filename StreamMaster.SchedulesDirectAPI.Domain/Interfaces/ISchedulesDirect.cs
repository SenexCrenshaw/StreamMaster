using StreamMaster.SchedulesDirectAPI.Domain.Models;
using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces;

public interface ISchedulesDirect
{
    bool CheckToken(bool forceReset = false);
    Task<HttpResponseMessage> GetSdImage(string uri);
    Task<List<SubscribedLineup>> GetLineups(CancellationToken cancellationToken);
    List<StationChannelName> GetStationChannelNames();
    Task<XMLTV?> CreateXmltv(string baseUrl, List<VideoStreamConfig> videoStreamConfigs);
    Task<StationChannelMap?> GetStationChannelMap(string lineup, CancellationToken cancellationToke);
    Task<bool> AddLineup(string lineup, CancellationToken cancellationToken);
    Task<List<ProgramMetadata>?> GetArtworkAsync(string[] request);
    Task<List<CountryData>?> GetAvailableCountries(CancellationToken cancellationToken);
    Task<List<string>?> GetCustomLogosFromServerAsync(string server);
    Task<Dictionary<string, GenericDescription>?> GetGenericDescriptionsAsync(string[] seriesIds, CancellationToken cancellationToken);
    Task<List<Headend>?> GetHeadends(string country, string postalCode, CancellationToken cancellationToken = default);
    //Task<bool> GetImageUrl(string programId, ProgramArtwork icon, CancellationToken cancellationToken);
    Task<List<LineupPreviewChannel>?> GetLineupPreviewChannel(string lineup, CancellationToken cancellationToken);
    Task<List<Programme>?> GetProgramsAsync(string[] programIds, CancellationToken cancellationToken);
    Task<List<ScheduleResponse>?> GetScheduleListingsAsync(ScheduleRequest[] request);
    Task<Dictionary<string, Dictionary<string, ScheduleMd5Response>>?> GetScheduleMd5sAsync(ScheduleRequest[] request, CancellationToken cancellationToken);
    Task<StationChannelMap?> GetStationChannelMapAsync(string lineup);
    Task<UserStatus> GetUserStatus(CancellationToken cancellationToken);
    Task<LineupResponse?> GetSubscribedLineups(CancellationToken cancellationToken);
    Task<bool> RemoveLineup(string lineup, CancellationToken cancellationToken);
    void ResetCache(string command);
    void ResetEPGCache();
    Task<bool> SDSync(CancellationToken cancellationToken);
    Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    Task<List<StationChannelMap>> GetStationChannelMaps(CancellationToken cancellationToken);

    //Task DownloadStationLogos(CancellationToken cancellationToken);
}