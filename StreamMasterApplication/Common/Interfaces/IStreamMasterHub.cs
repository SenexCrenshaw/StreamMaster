﻿using StreamMasterApplication.ChannelGroups.Commands;
using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

namespace StreamMasterApplication.Common.Interfaces;

public interface IStreamMasterHub : ISharedHub
{
    Task BroadcastStartUpData();

    Task ChannelGroupDtoDelete(int result);

    Task ChannelGroupDtoesUpdate(IEnumerable<ChannelGroupDto> channelGroupDto);

    Task ChannelGroupDtoUpdate(ChannelGroupDto channelGroupDto);

    Task ChannelGroupSetChannelGroupsVisible(IEnumerable<SetChannelGroupsVisibleArg> results);

    Task EPGFilesDtoDelete(int result);

    Task EPGFilesDtosUpdate(IEnumerable<EPGFilesDto> results);

    Task EPGFilesDtoUpdate(EPGFilesDto result);

    Task IconFileDTODelete(int result);

    Task IconFileDTOesUpdate(IEnumerable<IconFileDto> results);

    Task IconFileDTOUpdate(IconFileDto result);

    Task M3UFilesDtoDelete(int result);

    Task M3UFilesDtosUpdate(IEnumerable<M3UFilesDto> results);

    Task M3UFilesDtoUpdate(M3UFilesDto result);

    Task ProgrammeNamesUpdate(IEnumerable<Programme> results);

    Task ScanEPGDirectorty();

    Task ScanM3UDirectorty();

    Task SettingsUpdate(SettingDto setting);

    Task StreamGroupDtoDelete(int result);

    Task StreamGroupDtoesUpdate(IEnumerable<StreamGroupDto> results);

    Task StreamGroupDtoUpdate(StreamGroupDto result);

    Task StreamingStatusDelete(int result);

    Task StreamingStatusDtoUpdate(StreamingStatusDto result);

    Task StreamStatisticsResultsUpdate(List<StreamStatisticsResult> result);

    Task SystemStatusUpdate(SystemStatus result);

    Task TaskQueueStatusDtoesUpdate(IEnumerable<TaskQueueStatusDto> results);

    Task VideoStreamDtoDelete(string result);

    Task VideoStreamDtoesUpdate(IEnumerable<VideoStreamDto> results);

    Task VideoStreamDtosDelete(List<string> result);

    Task VideoStreamDtoUpdate(VideoStreamDto result);

    Task VideoStreamSetVideoStreamVisible(IEnumerable<SetVideoStreamVisibleRet> results);

    Task VideoStreamUpdateChannelNumbers(IEnumerable<ChannelNumberPair> data);
}
