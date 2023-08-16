using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamMasterHub : ISharedHub
{
    Task BroadcastStartUpData();
    Task ChannelGroupsRefresh();
    Task EPGFilesRefresh();
    Task M3UFilesRefresh();
    Task IconsRefresh();
    Task ProgrammeNamesUpdate(IEnumerable<Programme> results);
    Task ScanEPGDirectorty();
    Task ScanM3UDirectorty();
    Task SettingsUpdate(SettingDto setting);
    Task StreamGroupsRefresh();
    Task StreamingStatusDelete(int result);
    Task StreamingStatusDtoUpdate(StreamingStatusDto result);
    Task StreamStatisticsResultsUpdate(List<StreamStatisticsResult> result);
    Task SystemStatusUpdate(SystemStatus result);
    Task TaskQueueStatusDtoesUpdate(IEnumerable<TaskQueueStatusDto> results);
    Task VideoStreamsRefresh();

}