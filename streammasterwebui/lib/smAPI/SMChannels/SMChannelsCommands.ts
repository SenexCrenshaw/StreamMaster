import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CopySMChannelRequest,CreateSMChannelFromStreamRequest,CreateSMChannelRequest,DeleteSMChannelRequest,DeleteSMChannelsFromParametersRequest,DeleteSMChannelsRequest,SetSMChannelEPGIdRequest,SetSMChannelGroupRequest,SetSMChannelLogoRequest,SetSMChannelNameRequest,SetSMChannelNumberRequest,UpdateSMChannelRequest,SMChannelDto,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedSMChannels = async (parameters: QueryStringParameters): Promise<PagedResponse<SMChannelDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<SMChannelDto>>('GetPagedSMChannels', parameters);
};

export const GetSMChannelNames = async (): Promise<string[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<string[]>('GetSMChannelNames');
};

export const CopySMChannel = async (request: CopySMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CopySMChannel', request);
};

export const CreateSMChannelFromStream = async (request: CreateSMChannelFromStreamRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateSMChannelFromStream', request);
};

export const CreateSMChannel = async (request: CreateSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateSMChannel', request);
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

export const UpdateSMChannel = async (request: UpdateSMChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateSMChannel', request);
};

