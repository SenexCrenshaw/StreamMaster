import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,ToggleSMChannelsVisibleByIdRequest,ToggleSMChannelVisibleByIdRequest,ToggleSMChannelVisibleByParametersRequest,ToggleSMStreamsVisibleByIdRequest,ToggleSMStreamVisibleByIdRequest,ToggleSMStreamVisibleByParametersRequest,SMStreamDto,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedSMStreams = async (parameters: QueryStringParameters): Promise<PagedResponse<SMStreamDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<SMStreamDto>>('GetPagedSMStreams', parameters);
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

export const ToggleSMStreamsVisibleById = async (request: ToggleSMStreamsVisibleByIdRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ToggleSMStreamsVisibleById', request);
};

export const ToggleSMStreamVisibleById = async (request: ToggleSMStreamVisibleByIdRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ToggleSMStreamVisibleById', request);
};

export const ToggleSMStreamVisibleByParameters = async (request: ToggleSMStreamVisibleByParametersRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ToggleSMStreamVisibleByParameters', request);
};

