import { hubConnection } from "../app/signalr";
import {
  type AddChannelGroupRequest,
  type AddEpgFileRequest,
  type AddM3UFileRequest,
  type AddStreamGroupRequest,
  type AddVideoStreamRequest,
  type AutoMatchIconToStreamsRequest,
  type ChangeEpgFileNameRequest,
  type ChangeM3UFileNameRequest,
  type ChangeVideoStreamChannelRequest,
  type ChannelGroupDto,
  type ChannelLogoDto,
  type ChannelNumberPair,
  type Countries,
  type DeleteChannelGroupRequest,
  type DeleteEpgFileRequest,
  type DeleteM3UFileRequest,
  type DeleteStreamGroupRequest,
  type DeleteVideoStreamRequest,
  type EpgFilesDto,
  type EpgGuide,
  type FailClientRequest,
  type GetLog,
  type HeadendDto,
  type IconFileDto,
  type LineUpPreview,
  type LineUpResult,
  type LineUpsResult,
  type LogEntryDto,
  type LogInRequest,
  type M3UFilesDto,
  type ProcessEpgFileRequest,
  type ProcessM3UFileRequest,
  type ProgrammeNameDto,
  type RefreshEpgFileRequest,
  type RefreshM3UFileRequest,
  type ReSetVideoStreamsLogoRequest,
  type Schedule,
  type SdStatus,
  type SetChannelGroupsVisibleArg,
  type SetChannelGroupsVisibleRequest,
  type SettingDto,
  type SetVideoStreamChannelNumbersRequest,
  type SetVideoStreamsLogoToEpgRequest,
  type Station,
  type StationPreview,
  type StreamGroupDto,
  type StreamStatisticsResult,
  type SystemStatus,
  type TaskQueueStatusDto,
  type UpdateChannelGroupOrderRequest,
  type UpdateChannelGroupRequest,
  type UpdateChannelGroupsRequest,
  type UpdateEpgFileRequest,
  type UpdateM3UFileRequest,
  type UpdateStreamGroupRequest,
  type UpdateVideoStreamRequest,
  type UpdateVideoStreamsRequest,
  type VideoStreamDto,
} from "./iptvApi";

export const AddChannelGroup = async (arg: AddChannelGroupRequest): Promise<ChannelGroupDto> => {
    const data = await hubConnection.invoke('AddChannelGroup',arg);

    return data;
};

export const DeleteChannelGroup = async (arg: DeleteChannelGroupRequest): Promise<number> => {
    const data = await hubConnection.invoke('DeleteChannelGroup',arg);

    return data;
};

export const GetChannelGroup = async (arg: number): Promise<ChannelGroupDto> => {
    const data = await hubConnection.invoke('GetChannelGroup',arg);

    return data;
};

export const GetChannelGroups = async (): Promise<ChannelGroupDto[]> => {
    const data = await hubConnection.invoke('GetChannelGroups');

    return data;
};

export const SetChannelGroupsVisible = async (arg: SetChannelGroupsVisibleRequest): Promise<SetChannelGroupsVisibleArg[]> => {
    const data = await hubConnection.invoke('SetChannelGroupsVisible',arg);

    return data;
};

export const UpdateChannelGroup = async (arg: UpdateChannelGroupRequest): Promise<ChannelGroupDto> => {
    const data = await hubConnection.invoke('UpdateChannelGroup',arg);

    return data;
};

export const UpdateChannelGroupOrder = async (arg: UpdateChannelGroupOrderRequest): Promise<ChannelGroupDto[]> => {
    const data = await hubConnection.invoke('UpdateChannelGroupOrder',arg);

    return data;
};

export const UpdateChannelGroups = async (arg: UpdateChannelGroupsRequest): Promise<ChannelGroupDto[]> => {
    const data = await hubConnection.invoke('UpdateChannelGroups',arg);

    return data;
};

export const GetIsSystemReady = async (): Promise<boolean> => {
    const data = await hubConnection.invoke('GetIsSystemReady');

    return data;
};

export const OnConnectedAsync = async (): Promise<void> => {
    await hubConnection.invoke('OnConnectedAsync');
};

export const AddEPGFile = async (arg: AddEpgFileRequest): Promise<EpgFilesDto> => {
    const data = await hubConnection.invoke('AddEPGFile',arg);

    return data;
};

export const ChangeEPGFileName = async (arg: ChangeEpgFileNameRequest): Promise<EpgFilesDto> => {
    const data = await hubConnection.invoke('ChangeEPGFileName',arg);

    return data;
};

export const DeleteEPGFile = async (arg: DeleteEpgFileRequest): Promise<number> => {
    const data = await hubConnection.invoke('DeleteEPGFile',arg);

    return data;
};

export const GetEPGFile = async (arg: number): Promise<EpgFilesDto> => {
    const data = await hubConnection.invoke('GetEPGFile',arg);

    return data;
};

export const GetEPGFiles = async (): Promise<EpgFilesDto[]> => {
    const data = await hubConnection.invoke('GetEPGFiles');

    return data;
};

export const ProcessEPGFile = async (arg: ProcessEpgFileRequest): Promise<EpgFilesDto> => {
    const data = await hubConnection.invoke('ProcessEPGFile',arg);

    return data;
};

export const RefreshEPGFile = async (arg: RefreshEpgFileRequest): Promise<EpgFilesDto> => {
    const data = await hubConnection.invoke('RefreshEPGFile',arg);

    return data;
};

export const ScanDirectoryForEPGFiles = async (): Promise<boolean> => {
    const data = await hubConnection.invoke('ScanDirectoryForEPGFiles');

    return data;
};

export const UpdateEPGFile = async (arg: UpdateEpgFileRequest): Promise<EpgFilesDto> => {
    const data = await hubConnection.invoke('UpdateEPGFile',arg);

    return data;
};

export const AutoMatchIconToStreams = async (arg: AutoMatchIconToStreamsRequest): Promise<void> => {
    await hubConnection.invoke('AutoMatchIconToStreams',arg);
};

export const GetIcon = async (arg: number): Promise<IconFileDto> => {
    const data = await hubConnection.invoke('GetIcon',arg);

    return data;
};

export const GetIcons = async (): Promise<IconFileDto[]> => {
    const data = await hubConnection.invoke('GetIcons');

    return data;
};

export const GetLogRequest = async (arg: GetLog): Promise<LogEntryDto[]> => {
    const data = await hubConnection.invoke('GetLogRequest',arg);

    return data;
};

export const AddM3UFile = async (arg: AddM3UFileRequest): Promise<M3UFilesDto> => {
    const data = await hubConnection.invoke('AddM3UFile',arg);

    return data;
};

export const ChangeM3UFileName = async (arg: ChangeM3UFileNameRequest): Promise<M3UFilesDto> => {
    const data = await hubConnection.invoke('ChangeM3UFileName',arg);

    return data;
};

export const DeleteM3UFile = async (arg: DeleteM3UFileRequest): Promise<number> => {
    const data = await hubConnection.invoke('DeleteM3UFile',arg);

    return data;
};

export const GetM3UFile = async (arg: number): Promise<M3UFilesDto> => {
    const data = await hubConnection.invoke('GetM3UFile',arg);

    return data;
};

export const GetM3UFiles = async (): Promise<M3UFilesDto[]> => {
    const data = await hubConnection.invoke('GetM3UFiles');

    return data;
};

export const ProcessM3UFile = async (arg: ProcessM3UFileRequest): Promise<M3UFilesDto> => {
    const data = await hubConnection.invoke('ProcessM3UFile',arg);

    return data;
};

export const RefreshM3UFile = async (arg: RefreshM3UFileRequest): Promise<M3UFilesDto> => {
    const data = await hubConnection.invoke('RefreshM3UFile',arg);

    return data;
};

export const ScanDirectoryForM3UFiles = async (): Promise<boolean> => {
    const data = await hubConnection.invoke('ScanDirectoryForM3UFiles');

    return data;
};

export const UpdateM3UFile = async (arg: UpdateM3UFileRequest): Promise<M3UFilesDto> => {
    const data = await hubConnection.invoke('UpdateM3UFile',arg);

    return data;
};

export const GetProgrammeNames = async (): Promise<ProgrammeNameDto[]> => {
    const data = await hubConnection.invoke('GetProgrammeNames');

    return data;
};

export const GetCountries = async (): Promise<Countries> => {
    const data = await hubConnection.invoke('GetCountries');

    return data;
};

export const GetHeadends = async (arg: string): Promise<HeadendDto[]> => {
    const data = await hubConnection.invoke('GetHeadends',arg);

    return data;
};

export const GetLineup = async (arg: string): Promise<LineUpResult> => {
    const data = await hubConnection.invoke('GetLineup',arg);

    return data;
};

export const GetLineupPreviews = async (): Promise<LineUpPreview[]> => {
    const data = await hubConnection.invoke('GetLineupPreviews');

    return data;
};

export const GetLineups = async (): Promise<LineUpsResult> => {
    const data = await hubConnection.invoke('GetLineups');

    return data;
};

export const GetSchedules = async (): Promise<Schedule[]> => {
    const data = await hubConnection.invoke('GetSchedules');

    return data;
};

export const GetStationPreviews = async (): Promise<StationPreview[]> => {
    const data = await hubConnection.invoke('GetStationPreviews');

    return data;
};

export const GetStations = async (): Promise<Station[]> => {
    const data = await hubConnection.invoke('GetStations');

    return data;
};

export const GetStatus = async (): Promise<SdStatus> => {
    const data = await hubConnection.invoke('GetStatus');

    return data;
};

export const GetQueueStatus = async (): Promise<TaskQueueStatusDto[]> => {
    const data = await hubConnection.invoke('GetQueueStatus');

    return data;
};

export const GetSetting = async (): Promise<SettingDto> => {
    const data = await hubConnection.invoke('GetSetting');

    return data;
};

export const GetSystemStatus = async (): Promise<SystemStatus> => {
    const data = await hubConnection.invoke('GetSystemStatus');

    return data;
};

export const LogIn = async (arg: LogInRequest): Promise<boolean> => {
    const data = await hubConnection.invoke('LogIn',arg);

    return data;
};

export const AddStreamGroup = async (arg: AddStreamGroupRequest): Promise<StreamGroupDto> => {
    const data = await hubConnection.invoke('AddStreamGroup',arg);

    return data;
};

export const DeleteStreamGroup = async (arg: DeleteStreamGroupRequest): Promise<number> => {
    const data = await hubConnection.invoke('DeleteStreamGroup',arg);

    return data;
};

export const FailClient = async (arg: FailClientRequest): Promise<void> => {
    await hubConnection.invoke('FailClient',arg);
};

export const GetAllStatisticsForAllUrls = async (): Promise<StreamStatisticsResult[]> => {
    const data = await hubConnection.invoke('GetAllStatisticsForAllUrls');

    return data;
};

export const GetStreamGroup = async (arg: number): Promise<StreamGroupDto> => {
    const data = await hubConnection.invoke('GetStreamGroup',arg);

    return data;
};

export const GetStreamGroupByStreamNumber = async (arg: number): Promise<StreamGroupDto> => {
    const data = await hubConnection.invoke('GetStreamGroupByStreamNumber',arg);

    return data;
};

export const GetStreamGroupEPGForGuide = async (arg: number): Promise<EpgGuide> => {
    const data = await hubConnection.invoke('GetStreamGroupEPGForGuide',arg);

    return data;
};

export const GetStreamGroups = async (): Promise<StreamGroupDto[]> => {
    const data = await hubConnection.invoke('GetStreamGroups');

    return data;
};

export const SimulateStreamFailure = async (arg: string): Promise<void> => {
    await hubConnection.invoke('SimulateStreamFailure',arg);
};

export const UpdateStreamGroup = async (arg: UpdateStreamGroupRequest): Promise<StreamGroupDto> => {
    const data = await hubConnection.invoke('UpdateStreamGroup',arg);

    return data;
};

export const AddVideoStream = async (arg: AddVideoStreamRequest): Promise<VideoStreamDto> => {
    const data = await hubConnection.invoke('AddVideoStream',arg);

    return data;
};

export const ChangeVideoStreamChannel = async (arg: ChangeVideoStreamChannelRequest): Promise<void> => {
    await hubConnection.invoke('ChangeVideoStreamChannel',arg);
};

export const DeleteVideoStream = async (arg: DeleteVideoStreamRequest): Promise<string> => {
    const data = await hubConnection.invoke('DeleteVideoStream',arg);

    return data;
};

export const GetChannelLogoDtos = async (): Promise<ChannelLogoDto[]> => {
    const data = await hubConnection.invoke('GetChannelLogoDtos');

    return data;
};

export const GetVideoStream = async (arg: string): Promise<VideoStreamDto> => {
    const data = await hubConnection.invoke('GetVideoStream',arg);

    return data;
};

export const GetVideoStreams = async (): Promise<VideoStreamDto[]> => {
    const data = await hubConnection.invoke('GetVideoStreams');

    return data;
};

export const ReSetVideoStreamsLogo = async (arg: ReSetVideoStreamsLogoRequest): Promise<void> => {
    await hubConnection.invoke('ReSetVideoStreamsLogo',arg);
};

export const SetVideoStreamChannelNumbers = async (arg: SetVideoStreamChannelNumbersRequest): Promise<ChannelNumberPair[]> => {
    const data = await hubConnection.invoke('SetVideoStreamChannelNumbers',arg);

    return data;
};

export const SetVideoStreamsLogoToEPG = async (arg: SetVideoStreamsLogoToEpgRequest): Promise<void> => {
    await hubConnection.invoke('SetVideoStreamsLogoToEPG',arg);
};

export const UpdateVideoStream = async (arg: UpdateVideoStreamRequest): Promise<VideoStreamDto> => {
    const data = await hubConnection.invoke('UpdateVideoStream',arg);

    return data;
};

export const UpdateVideoStreams = async (arg: UpdateVideoStreamsRequest): Promise<VideoStreamDto[]> => {
    const data = await hubConnection.invoke('UpdateVideoStreams',arg);

    return data;
};

