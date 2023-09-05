using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamMasterHub : ISharedHub
{
    Task BroadcastStartUpData();
    Task ChannelGroupsRefresh();
    Task EPGFilesRefresh();
    Task M3UFilesRefresh();
    Task IconsRefresh();
    Task ProgrammesRefresh();
    Task StreamGroupsRefresh();
    Task SettingsUpdate(SettingDto setting);
    Task StreamingStatusDtoUpdate(StreamingStatusDto result);
    Task StreamStatisticsResultsUpdate(List<StreamStatisticsResult> result);
    Task SystemStatusUpdate(SystemStatus result);
    Task TaskQueueStatusDtoesUpdate(IEnumerable<TaskQueueStatusDto> results);
    Task VideoStreamsRefresh();

}