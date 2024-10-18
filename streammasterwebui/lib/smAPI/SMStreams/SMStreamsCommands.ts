import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CreateSMStreamRequest,DeleteSMStreamRequest,SetSMStreamsVisibleByIdRequest,ToggleSMStreamsVisibleByIdRequest,ToggleSMStreamVisibleByIdRequest,ToggleSMStreamVisibleByParametersRequest,UpdateSMStreamRequest,SMStreamDto,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedSMStreams = async (parameters: QueryStringParameters): Promise<PagedResponse<SMStreamDto> | undefined> => {
  if (isSkipToken(parameters) || parameters === undefined) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<SMStreamDto>>('GetPagedSMStreams', parameters);
};

export const CreateSMStream = async (request: CreateSMStreamRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateSMStream', request);
};

export const DeleteSMStream = async (request: DeleteSMStreamRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteSMStream', request);
};

export const SetSMStreamsVisibleById = async (request: SetSMStreamsVisibleByIdRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('SetSMStreamsVisibleById', request);
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

export const UpdateSMStream = async (request: UpdateSMStreamRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateSMStream', request);
};

