import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CreateM3UFileRequest,DeleteM3UFileRequest,ProcessM3UFileRequest,ProcessM3UFilesRequest,RefreshM3UFileRequest,UpdateM3UFileRequest,M3UFileDto,GetPagedM3UFilesRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedM3UFiles = async (parameters: QueryStringParameters): Promise<PagedResponse<M3UFileDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<M3UFileDto>>('GetPagedM3UFiles', parameters);
};

export const CreateM3UFile = async (request: CreateM3UFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CreateM3UFile', request);
};

export const DeleteM3UFile = async (request: DeleteM3UFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('DeleteM3UFile', request);
};

export const ProcessM3UFile = async (request: ProcessM3UFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ProcessM3UFile', request);
};

export const ProcessM3UFiles = async (): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ProcessM3UFiles');
};

export const RefreshM3UFile = async (request: RefreshM3UFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('RefreshM3UFile', request);
};

export const UpdateM3UFile = async (request: UpdateM3UFileRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('UpdateM3UFile', request);
};

