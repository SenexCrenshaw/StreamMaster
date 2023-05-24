using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamMasterHub : ISharedHub
{
    Task BroadcastStartUpData();

    Task ChannelGroupDtoDelete(int result);

    Task ChannelGroupDtoesUpdate(IEnumerable<ChannelGroupDto> channelGroupDto);

    Task ChannelGroupDtoUpdate(ChannelGroupDto channelGroupDto);

    Task ChannelGroupSetChannelGroupsVisible(IEnumerable<SetChannelGroupsVisibleArg> results);

    Task EPGFilesDtoDelete(int result);

    //Task ChannelCategoryUpdate(ChannelCategoryDto result);
    Task EPGFilesDtosUpdate(IEnumerable<EPGFilesDto> results);

    //// Task BroadcastUpdateStreamGroups(StreamGroupDto data);
    //Task ChannelCategoryDelete(int result);
    Task EPGFilesDtoUpdate(EPGFilesDto result);

    //Task ChannelCategoriesUpdate(IEnumerable<ChannelCategoryDto> results);
    Task IconFileDTODelete(int result);

    //Task ExtendedVideoStreamUpdateChannelNumbers(IEnumerable<ChannelNumberPair> data);
    Task IconFileDTOesUpdate(IEnumerable<IconFileDto> results);

    //Task ExtendedVideoStreamDtoUpdate(ExtendedVideoStreamDto result);
    Task IconFileDTOUpdate(IconFileDto result);

    //Task IPTVChannelDtoUpdate(IPTVChannelDto result);
    Task M3UFilesDtoDelete(int result);

    //Task IPTVChannelDtoesUpdate(IEnumerable<IPTVChannelDto> results);
    Task M3UFilesDtosUpdate(IEnumerable<M3UFilesDto> results);

    //Task ExtendedVideoStreamDtoesUpdate(IEnumerable<ExtendedVideoStreamDto> results);
    //Task IPTVChannelDtoDelete(int result);
    Task M3UFilesDtoUpdate(M3UFilesDto result);

    Task ScanEPGDirectorty();

    //Task M3UStreamUpdateChannelNumbers(IEnumerable<ChannelNumberPair> data);
    Task ScanM3UDirectorty();

    //Task M3UStreamDtoUpdate(M3UStreamDto result);
    Task SettingsUpdate(SettingDto setting);

    //Task M3UStreamDtosUpdate(IEnumerable<M3UStreamDto> results);
    Task StreamGroupDtoDelete(int result);

    //Task ExtendedVideoStreamDtoDelete(int result);
    //Task M3UStreamDtoDelete(int result);
    Task StreamGroupDtoesUpdate(IEnumerable<StreamGroupDto> results);

    Task StreamGroupDtoUpdate(StreamGroupDto result);

    Task StreamingStatusDelete(int result);

    Task StreamingStatusDtoUpdate(StreamingStatusDto result);

    Task StreamStatisticsResultsUpdate(List<StreamStatisticsResult> result);

    Task SystemStatusUpdate(SystemStatus result);

    Task TaskQueueStatusDtoesUpdate(IEnumerable<TaskQueueStatusDto> results);

    Task VideoStreamDtoDelete(int result);

    Task VideoStreamDtoesUpdate(IEnumerable<VideoStreamDto> results);

    Task VideoStreamDtoUpdate(VideoStreamDto result);

    Task VideoStreamSetVideoStreamVisible(IEnumerable<SetVideoStreamVisibleRet> results);

    Task VideoStreamUpdateChannelNumbers(IEnumerable<ChannelNumberPair> data);

    //Task StreamingStatusesUpdate(StreamingStatusDto results);
}
