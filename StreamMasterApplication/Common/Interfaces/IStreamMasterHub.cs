using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Queries;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamMasterHub : ISharedHub
{
    Task VideoStreamLinksRemove(string[]? results = null);
    Task VideoStreamLinksRefresh(string[]? results = null);
    Task BroadcastStartUpData();
    Task ChannelGroupsRefresh(ChannelGroupDto[]? results = null);
    Task EPGFilesRefresh(EPGFileDto[]? results = null);
    Task M3UFilesRefresh(M3UFileDto[]? results = null);
    Task IconsRefresh();
    Task ProgrammesRefresh();
    Task SchedulesDirectsRefresh();
    Task StreamGroupsRefresh(StreamGroupDto[]? results = null);
    Task StreamGroupVideoStreamsRefresh(StreamGroupVideoStream[]? results = null);
    Task StreamGroupChannelGroupsRefresh(StreamGroupChannelGroup[]? results = null);
    Task SettingsUpdate(SettingDto setting);
    Task StreamingStatusDtoUpdate(StreamingStatusDto result);
    Task StreamStatisticsResultsUpdate(List<StreamStatisticsResult> result);
    Task SystemStatusUpdate(SDSystemStatus result);
    Task TaskQueueStatusDtoesUpdate(IEnumerable<TaskQueueStatusDto> results);
    Task VideoStreamsRefresh(VideoStreamDto[]? results = null);
    Task ChannelGroupCreated(ChannelGroupDto channelGroup);
    Task ChannelGroupDelete(int ChannelGroupId);
    Task ChannelGroupsDelete(IEnumerable<int> ChannelGroupIds);
    Task VideoStreamsVisibilityRefresh(IEnumerable<IDIsHidden> results);
    Task UpdateChannelGroupVideoStreamCounts(List<ChannelGroupStreamCount> channelGroupStreamCounts);
}