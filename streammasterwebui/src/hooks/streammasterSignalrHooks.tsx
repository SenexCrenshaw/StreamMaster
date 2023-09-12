import { hubConnection } from "../app/signalr";
import type * as iptv from "../store/iptvApi";


export const CreateChannelGroup = async (arg: iptv.CreateChannelGroupRequest): Promise<void> => {
  await hubConnection.invoke('CreateChannelGroup', arg);
};

export const DeleteAllChannelGroupsFromParameters = async (arg: iptv.DeleteAllChannelGroupsFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('DeleteAllChannelGroupsFromParameters', arg);
};

export const DeleteChannelGroup = async (arg: iptv.DeleteChannelGroupRequest): Promise<void> => {
  await hubConnection.invoke('DeleteChannelGroup', arg);
};

export const GetChannelGroup = async (): Promise<iptv.ChannelGroupDto> => {
  const data = await hubConnection.invoke('GetChannelGroup', );
  return data;
};

export const GetChannelGroupIdNames = async (): Promise<iptv.ChannelGroupIdName[]> => {
  const data = await hubConnection.invoke('GetChannelGroupIdNames', );
  return data;
};

export const GetChannelGroups = async (): Promise<iptv.PagedResponseOfChannelGroupDto> => {
  const data = await hubConnection.invoke('GetChannelGroups', );
  return data;
};

export const UpdateChannelGroup = async (arg: iptv.UpdateChannelGroupRequest): Promise<void> => {
  await hubConnection.invoke('UpdateChannelGroup', arg);
};

export const UpdateChannelGroups = async (arg: iptv.UpdateChannelGroupsRequest): Promise<void> => {
  await hubConnection.invoke('UpdateChannelGroups', arg);
};

export const GetChannelGroupNames = async (): Promise<void> => {
  await hubConnection.invoke('GetChannelGroupNames', );
};

export const CreateEpgFile = async (arg: iptv.CreateEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('CreateEpgFile', arg);
};

export const CreateEpgFileFromForm = async (): Promise<void> => {
  await hubConnection.invoke('CreateEpgFileFromForm', );
};

export const DeleteEpgFile = async (arg: iptv.DeleteEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('DeleteEpgFile', arg);
};

export const GetEpgFile = async (): Promise<iptv.EpgFileDto> => {
  const data = await hubConnection.invoke('GetEpgFile', );
  return data;
};

export const GetEpgFiles = async (): Promise<iptv.PagedResponseOfEpgFileDto> => {
  const data = await hubConnection.invoke('GetEpgFiles', );
  return data;
};

export const ProcessEpgFile = async (arg: iptv.ProcessEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('ProcessEpgFile', arg);
};

export const RefreshEpgFile = async (arg: iptv.RefreshEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('RefreshEpgFile', arg);
};

export const ScanDirectoryForEpgFiles = async (): Promise<void> => {
  await hubConnection.invoke('ScanDirectoryForEpgFiles', );
};

export const UpdateEpgFile = async (arg: iptv.UpdateEpgFileRequest): Promise<void> => {
  await hubConnection.invoke('UpdateEpgFile', arg);
};

export const GetFile = async (arg: iptv.SmFileTypes): Promise<void> => {
  await hubConnection.invoke('GetFile', arg);
};

export const AutoMatchIconToStreams = async (arg: iptv.AutoMatchIconToStreamsRequest): Promise<void> => {
  await hubConnection.invoke('AutoMatchIconToStreams', arg);
};

export const GetIcon = async (): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIcon', );
  return data;
};

export const GetIconFromSource = async (): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIconFromSource', );
  return data;
};

export const GetIcons = async (): Promise<iptv.PagedResponseOfIconFileDto> => {
  const data = await hubConnection.invoke('GetIcons', );
  return data;
};

export const GetIconsSimpleQuery = async (): Promise<iptv.IconFileDto[]> => {
  const data = await hubConnection.invoke('GetIconsSimpleQuery', );
  return data;
};

export const GetLogRequest = async (arg: iptv.GetLog): Promise<iptv.LogEntryDto[]> => {
  const data = await hubConnection.invoke('GetLogRequest', arg);
  return data;
};

export const CreateM3UFile = async (arg: iptv.CreateM3UFileRequest): Promise<void> => {
  await hubConnection.invoke('CreateM3UFile', arg);
};

export const CreateM3UFileFromForm = async (): Promise<void> => {
  await hubConnection.invoke('CreateM3UFileFromForm', );
};

export const ChangeM3UFileName = async (arg: iptv.ChangeM3UFileNameRequest): Promise<void> => {
  await hubConnection.invoke('ChangeM3UFileName', arg);
};

export const DeleteM3UFile = async (arg: iptv.DeleteM3UFileRequest): Promise<void> => {
  await hubConnection.invoke('DeleteM3UFile', arg);
};

export const GetM3UFile = async (): Promise<iptv.M3UFileDto> => {
  const data = await hubConnection.invoke('GetM3UFile', );
  return data;
};

export const GetM3UFiles = async (): Promise<iptv.PagedResponseOfM3UFileDto> => {
  const data = await hubConnection.invoke('GetM3UFiles', );
  return data;
};

export const ProcessM3UFile = async (arg: iptv.ProcessM3UFileRequest): Promise<void> => {
  await hubConnection.invoke('ProcessM3UFile', arg);
};

export const RefreshM3UFile = async (arg: iptv.RefreshM3UFileRequest): Promise<void> => {
  await hubConnection.invoke('RefreshM3UFile', arg);
};

export const ScanDirectoryForM3UFiles = async (): Promise<void> => {
  await hubConnection.invoke('ScanDirectoryForM3UFiles', );
};

export const UpdateM3UFile = async (arg: iptv.UpdateM3UFileRequest): Promise<void> => {
  await hubConnection.invoke('UpdateM3UFile', arg);
};

export const GetM3UFileNames = async (): Promise<void> => {
  await hubConnection.invoke('GetM3UFileNames', );
};

export const BuildIconsCacheFromVideoStreams = async (): Promise<void> => {
  await hubConnection.invoke('BuildIconsCacheFromVideoStreams', );
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void> => {
  await hubConnection.invoke('BuildProgIconsCacheFromEpgsRequest', );
};

export const GetProgramme = async (): Promise<iptv.Programme[]> => {
  const data = await hubConnection.invoke('GetProgramme', );
  return data;
};

export const GetProgrammeChannels = async (): Promise<iptv.ProgrammeChannel[]> => {
  const data = await hubConnection.invoke('GetProgrammeChannels', );
  return data;
};

export const GetProgrammeNameSelections = async (): Promise<iptv.PagedResponseOfProgrammeNameDto> => {
  const data = await hubConnection.invoke('GetProgrammeNameSelections', );
  return data;
};

export const GetProgrammes = async (): Promise<iptv.Programme[]> => {
  const data = await hubConnection.invoke('GetProgrammes', );
  return data;
};

export const GetProgrammeNames = async (): Promise<void> => {
  await hubConnection.invoke('GetProgrammeNames', );
};

export const GetProgrammsSimpleQuery = async (): Promise<iptv.ProgrammeNameDto[]> => {
  const data = await hubConnection.invoke('GetProgrammsSimpleQuery', );
  return data;
};

export const GetProgrammeFromDisplayName = async (): Promise<iptv.ProgrammeNameDto> => {
  const data = await hubConnection.invoke('GetProgrammeFromDisplayName', );
  return data;
};

export const GetCountries = async (): Promise<iptv.Countries> => {
  const data = await hubConnection.invoke('GetCountries', );
  return data;
};

export const GetHeadends = async (): Promise<iptv.HeadendDto[]> => {
  const data = await hubConnection.invoke('GetHeadends', );
  return data;
};

export const GetLineup = async (): Promise<iptv.LineUpResult> => {
  const data = await hubConnection.invoke('GetLineup', );
  return data;
};

export const GetLineupPreviews = async (): Promise<iptv.LineUpPreview[]> => {
  const data = await hubConnection.invoke('GetLineupPreviews', );
  return data;
};

export const GetLineups = async (): Promise<iptv.LineUpsResult> => {
  const data = await hubConnection.invoke('GetLineups', );
  return data;
};

export const GetSchedules = async (): Promise<iptv.Schedule[]> => {
  const data = await hubConnection.invoke('GetSchedules', );
  return data;
};

export const GetStationPreviews = async (): Promise<iptv.StationPreview[]> => {
  const data = await hubConnection.invoke('GetStationPreviews', );
  return data;
};

export const GetStations = async (): Promise<iptv.Station[]> => {
  const data = await hubConnection.invoke('GetStations', );
  return data;
};

export const GetStatus = async (): Promise<iptv.SdStatus> => {
  const data = await hubConnection.invoke('GetStatus', );
  return data;
};

export const GetIsSystemReady = async (): Promise<void> => {
  await hubConnection.invoke('GetIsSystemReady', );
};

export const GetQueueStatus = async (): Promise<iptv.TaskQueueStatusDto[]> => {
  const data = await hubConnection.invoke('GetQueueStatus', );
  return data;
};

export const GetSetting = async (): Promise<iptv.SettingDto> => {
  const data = await hubConnection.invoke('GetSetting', );
  return data;
};

export const GetSystemStatus = async (): Promise<iptv.SystemStatus> => {
  const data = await hubConnection.invoke('GetSystemStatus', );
  return data;
};

export const LogIn = async (arg: iptv.LogInRequest): Promise<void> => {
  await hubConnection.invoke('LogIn', arg);
};

export const UpdateSetting = async (arg: iptv.UpdateSettingRequest): Promise<void> => {
  await hubConnection.invoke('UpdateSetting', arg);
};

export const SyncStreamGroupChannelGroups = async (arg: iptv.SyncStreamGroupChannelGroupsRequest): Promise<iptv.StreamGroupDto> => {
  const data = await hubConnection.invoke('SyncStreamGroupChannelGroups', arg);
  return data;
};

export const GetChannelGroupsFromStreamGroup = async (): Promise<iptv.ChannelGroupDto[]> => {
  const data = await hubConnection.invoke('GetChannelGroupsFromStreamGroup', );
  return data;
};

export const GetAllChannelGroups = async (): Promise<iptv.ChannelGroupDto[]> => {
  const data = await hubConnection.invoke('GetAllChannelGroups', );
  return data;
};

export const CreateStreamGroup = async (arg: iptv.CreateStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('CreateStreamGroup', arg);
};

export const DeleteStreamGroup = async (arg: iptv.DeleteStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('DeleteStreamGroup', arg);
};

export const GetStreamGroup = async (): Promise<iptv.StreamGroupDto> => {
  const data = await hubConnection.invoke('GetStreamGroup', );
  return data;
};

export const GetStreamGroupCapability = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupCapability', );
};

export const GetStreamGroupCapability2 = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupCapability2', );
};

export const GetStreamGroupCapability3 = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupCapability3', );
};

export const GetStreamGroupDiscover = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupDiscover', );
};

export const GetStreamGroupEpg = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupEpg', );
};

export const GetStreamGroupEpgForGuide = async (): Promise<iptv.EpgGuide> => {
  const data = await hubConnection.invoke('GetStreamGroupEpgForGuide', );
  return data;
};

export const GetStreamGroupLineUp = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupLineUp', );
};

export const GetStreamGroupLineUpStatus = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupLineUpStatus', );
};

export const GetStreamGroupM3U = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupM3U', );
};

export const GetStreamGroups = async (): Promise<iptv.PagedResponseOfStreamGroupDto> => {
  const data = await hubConnection.invoke('GetStreamGroups', );
  return data;
};

export const UpdateStreamGroup = async (arg: iptv.UpdateStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('UpdateStreamGroup', arg);
};

export const GetStreamGroupVideoStreamIds = async (): Promise<iptv.VideoStreamIsReadOnly[]> => {
  const data = await hubConnection.invoke('GetStreamGroupVideoStreamIds', );
  return data;
};

export const GetStreamGroupVideoStreams = async (): Promise<iptv.PagedResponseOfVideoStreamDto> => {
  const data = await hubConnection.invoke('GetStreamGroupVideoStreams', );
  return data;
};

export const SetVideoStreamRanks = async (arg: iptv.SetVideoStreamRanksRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamRanks', arg);
};

export const SyncVideoStreamToStreamGroupPOST = async (arg: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('SyncVideoStreamToStreamGroupPOST', arg);
};

export const SyncVideoStreamToStreamGroupDELETE = async (arg: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('SyncVideoStreamToStreamGroupDELETE', arg);
};

export const AddVideoStreamToVideoStream = async (arg: iptv.AddVideoStreamToVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('AddVideoStreamToVideoStream', arg);
};

export const GetVideoStreamVideoStreamIds = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamVideoStreamIds', );
};

export const GetVideoStreamVideoStreams = async (): Promise<iptv.PagedResponseOfChildVideoStreamDto> => {
  const data = await hubConnection.invoke('GetVideoStreamVideoStreams', );
  return data;
};

export const RemoveVideoStreamFromVideoStream = async (arg: iptv.RemoveVideoStreamFromVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('RemoveVideoStreamFromVideoStream', arg);
};

export const CreateVideoStream = async (arg: iptv.CreateVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('CreateVideoStream', arg);
};

export const ChangeVideoStreamChannel = async (arg: iptv.ChangeVideoStreamChannelRequest): Promise<void> => {
  await hubConnection.invoke('ChangeVideoStreamChannel', arg);
};

export const DeleteVideoStream = async (arg: iptv.DeleteVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('DeleteVideoStream', arg);
};

export const FailClient = async (arg: iptv.FailClientRequest): Promise<void> => {
  await hubConnection.invoke('FailClient', arg);
};

export const GetAllStatisticsForAllUrls = async (): Promise<iptv.StreamStatisticsResult[]> => {
  const data = await hubConnection.invoke('GetAllStatisticsForAllUrls', );
  return data;
};

export const GetChannelLogoDtos = async (): Promise<iptv.ChannelLogoDto[]> => {
  const data = await hubConnection.invoke('GetChannelLogoDtos', );
  return data;
};

export const GetVideoStream = async (): Promise<iptv.VideoStreamDto> => {
  const data = await hubConnection.invoke('GetVideoStream', );
  return data;
};

export const GetVideoStreams = async (): Promise<iptv.PagedResponseOfVideoStreamDto> => {
  const data = await hubConnection.invoke('GetVideoStreams', );
  return data;
};

export const GetVideoStreamStream = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream', );
};

export const GetVideoStreamStream2 = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream2', );
};

export const GetVideoStreamStream3 = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream3', );
};

export const ReSetVideoStreamsLogo = async (arg: iptv.ReSetVideoStreamsLogoRequest): Promise<void> => {
  await hubConnection.invoke('ReSetVideoStreamsLogo', arg);
};

export const SetVideoStreamChannelNumbers = async (arg: iptv.SetVideoStreamChannelNumbersRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamChannelNumbers', arg);
};

export const SetVideoStreamsLogoFromEpg = async (arg: iptv.SetVideoStreamsLogoFromEpgRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamsLogoFromEpg', arg);
};

export const UpdateVideoStream = async (arg: iptv.UpdateVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('UpdateVideoStream', arg);
};

export const UpdateVideoStreams = async (arg: iptv.UpdateVideoStreamsRequest): Promise<void> => {
  await hubConnection.invoke('UpdateVideoStreams', arg);
};

export const UpdateAllVideoStreamsFromParameters = async (arg: iptv.UpdateAllVideoStreamsFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('UpdateAllVideoStreamsFromParameters', arg);
};

export const DeleteAllVideoStreamsFromParameters = async (arg: iptv.DeleteAllVideoStreamsFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('DeleteAllVideoStreamsFromParameters', arg);
};

export const SetVideoStreamChannelNumbersFromParameters = async (arg: iptv.SetVideoStreamChannelNumbersFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamChannelNumbersFromParameters', arg);
};

export const SetVideoStreamsLogoFromEpgFromParameters = async (arg: iptv.SetVideoStreamsLogoFromEpgFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamsLogoFromEpgFromParameters', arg);
};

export const ReSetVideoStreamsLogoFromParameters = async (arg: iptv.ReSetVideoStreamsLogoFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('ReSetVideoStreamsLogoFromParameters', arg);
};

export const SimulateStreamFailureForAll = async (): Promise<void> => {
  await hubConnection.invoke('SimulateStreamFailureForAll', );
};

export const SimulateStreamFailure = async (arg: iptv.SimulateStreamFailureRequest): Promise<void> => {
  await hubConnection.invoke('SimulateStreamFailure', arg);
};
