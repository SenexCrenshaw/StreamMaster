import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,AutoSetEPGFromParametersRequest,AutoSetEPGRequest,AutoSetSMChannelNumbersFromParametersRequest,AutoSetSMChannelNumbersRequest,CopySMChannelRequest,CreateSMChannelRequest,CreateSMChannelsFromStreamParametersRequest,CreateSMChannelsFromStreamsRequest,DeleteSMChannelRequest,DeleteSMChannelsFromParametersRequest,DeleteSMChannelsRequest,SetSMChannelEPGIdRequest,SetSMChannelGroupRequest,SetSMChannelLogoRequest,SetSMChannelNameRequest,SetSMChannelNumberRequest,SetSMChannelsGroupFromParametersRequest,SetSMChannelsGroupRequest,SetSMChannelsLogoFromEPGFromParametersRequest,SetSMChannelsLogoFromEPGRequest,SetSMChannelsVideoOutputProfileNameFromParametersRequest,SetSMChannelsVideoOutputProfileNameRequest,SetSMChannelVideoOutputProfileNameRequest,ToggleSMChannelsVisibleByIdRequest,ToggleSMChannelVisibleByIdRequest,ToggleSMChannelVisibleByParametersRequest,UpdateSMChannelRequest,SMChannelDto,VideoInfo,GetSMChannelRequest,GetVideoInfoFromIdRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedSMChannels = async (parameters: QueryStringParameters): Promise<PagedResponse<SMChannelDto> | undefined> => {
  if (isSkipToken(parameters) || parameters === undefined) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<SMChannelDto>>('GetPagedSMChannels', parameters);
};

export const GetSMChannelNames = async (): Promise<string[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<string[]>('GetSMChannelNames');
};

export const GetSMChannel = async (request: GetSMChannelRequest): Promise<SMChannelDto | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<SMChannelDto>('GetSMChannel', request);
};

export const GetVideoInfoFromId = async (request: GetVideoInfoFromIdRequest): Promise<VideoInfo | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<VideoInfo>('GetVideoInfoFromId', request);
};

export const AutoSetEPGFromParameters = async (request: AutoSetEPGFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AutoSetEPGFromParameters', request);
};

export const AutoSetEPG = async (request: AutoSetEPGRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AutoSetEPG', request);
};

export const AutoSetSMChannelNumbersFromParameters = async (request: AutoSetSMChannelNumbersFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AutoSetSMChannelNumbersFromParameters', request);
};

export const AutoSetSMChannelNumbers = async (request: AutoSetSMChannelNumbersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('AutoSetSMChannelNumbers', request);
};

export const CopySMChannel = async (request: CopySMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CopySMChannel', request);
};

export const CreateSMChannel = async (request: CreateSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateSMChannel', request);
};

export const CreateSMChannelsFromStreamParameters = async (request: CreateSMChannelsFromStreamParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateSMChannelsFromStreamParameters', request);
};

export const CreateSMChannelsFromStreams = async (request: CreateSMChannelsFromStreamsRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateSMChannelsFromStreams', request);
};

export const DeleteSMChannel = async (request: DeleteSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteSMChannel', request);
};

export const DeleteSMChannelsFromParameters = async (request: DeleteSMChannelsFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteSMChannelsFromParameters', request);
};

export const DeleteSMChannels = async (request: DeleteSMChannelsRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteSMChannels', request);
};

export const SetSMChannelEPGId = async (request: SetSMChannelEPGIdRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelEPGId', request);
};

export const SetSMChannelGroup = async (request: SetSMChannelGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelGroup', request);
};

export const SetSMChannelLogo = async (request: SetSMChannelLogoRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelLogo', request);
};

export const SetSMChannelName = async (request: SetSMChannelNameRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelName', request);
};

export const SetSMChannelNumber = async (request: SetSMChannelNumberRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelNumber', request);
};

export const SetSMChannelsGroupFromParameters = async (request: SetSMChannelsGroupFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsGroupFromParameters', request);
};

export const SetSMChannelsGroup = async (request: SetSMChannelsGroupRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsGroup', request);
};

export const SetSMChannelsLogoFromEPGFromParameters = async (request: SetSMChannelsLogoFromEPGFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsLogoFromEPGFromParameters', request);
};

export const SetSMChannelsLogoFromEPG = async (request: SetSMChannelsLogoFromEPGRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsLogoFromEPG', request);
};

export const SetSMChannelsVideoOutputProfileNameFromParameters = async (request: SetSMChannelsVideoOutputProfileNameFromParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsVideoOutputProfileNameFromParameters', request);
};

export const SetSMChannelsVideoOutputProfileName = async (request: SetSMChannelsVideoOutputProfileNameRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelsVideoOutputProfileName', request);
};

export const SetSMChannelVideoOutputProfileName = async (request: SetSMChannelVideoOutputProfileNameRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMChannelVideoOutputProfileName', request);
};

export const ToggleSMChannelsVisibleById = async (request: ToggleSMChannelsVisibleByIdRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ToggleSMChannelsVisibleById', request);
};

export const ToggleSMChannelVisibleById = async (request: ToggleSMChannelVisibleByIdRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ToggleSMChannelVisibleById', request);
};

export const ToggleSMChannelVisibleByParameters = async (request: ToggleSMChannelVisibleByParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ToggleSMChannelVisibleByParameters', request);
};

export const UpdateSMChannel = async (request: UpdateSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateSMChannel', request);
};

