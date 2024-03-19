using StreamMaster.Application.Common.Models;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.Common.Interfaces;

public interface IStreamMasterHub : ISharedHub
{
    Task<DefaultAPIResponse> ToggleSMStreamVisibleRequest(string Id, CancellationToken cancellationToken);

    Task VideoStreamLinksRemove(string[]? results = null);
    Task VideoStreamLinksRefresh(string[]? results = null);
    Task BroadcastStartUpData();
    Task MiscRefresh();
    Task ChannelGroupsRefresh(ChannelGroupDto[]? results = null);
    Task EPGFilesRefresh(EPGFileDto[]? results = null);
    Task M3UFilesRefresh(M3UFileDto[]? results = null);
    Task IconsRefresh();
    Task ProgrammesRefresh();
    Task CacheHandler(string cacheType);
    Task StreamsRefresh(SMStreamDto[]? results = null);
    Task SchedulesDirectsRefresh();
    Task StreamGroupsRefresh(StreamGroupDto[]? results = null);
    Task StreamGroupVideoStreamsRefresh(StreamGroupVideoStream[]? results = null);
    Task StreamGroupChannelGroupsRefresh(StreamGroupChannelGroup[]? results = null);
    Task SettingsUpdate(SettingDto setting);
    Task StreamingStatusDtoUpdate(StreamingStatusDto result);
    Task ClientStreamingStatisticsUpdate(List<ClientStreamingStatistics> result);
    Task InputStreamingStatisticsUpdate(List<InputStreamingStatistics> result);
    Task SystemStatusUpdate(SDSystemStatus result);
    Task TaskQueueStatusUpdate(IEnumerable<TaskQueueStatus> results);
    Task VideoStreamsRefresh(VideoStreamDto[]? results = null);
    Task ChannelGroupCreated(ChannelGroupDto channelGroup);
    Task ChannelGroupDelete(int ChannelGroupId);
    Task ChannelGroupsDelete(IEnumerable<int> ChannelGroupIds);
    Task VideoStreamsVisibilityRefresh(IEnumerable<IDIsHidden> results);
    Task UpdateChannelGroupVideoStreamCounts(List<ChannelGroupStreamCount> channelGroupStreamCounts);
    Task SetField(FieldData[] fieldData);
}