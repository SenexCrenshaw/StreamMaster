import SignalRService from '@lib/signalr/SignalRService';
import { DefaultAPIResponse,CreateM3UFileRequest,DeleteM3UFileRequest,ProcessM3UFileRequest,RefreshM3UFileRequest,UpdateM3UFileRequest,M3UFileDto,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedM3UFiles = async (parameters: QueryStringParameters): Promise<PagedResponse<M3UFileDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse<M3UFileDto>>('GetPagedM3UFiles', parameters)
    .then((response) => {
      if (response) {
        return response.pagedResponse;
      }
      return undefined;
    })
    .catch((error) => {
      console.error(error);
      return undefined;
    });
};

export const CreateM3UFile = async (request: CreateM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateM3UFile', request);
};

export const DeleteM3UFile = async (request: DeleteM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteM3UFile', request);
};

export const ProcessM3UFile = async (request: ProcessM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('ProcessM3UFile', request);
};

export const RefreshM3UFile = async (request: RefreshM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('RefreshM3UFile', request);
};

export const UpdateM3UFile = async (request: UpdateM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('UpdateM3UFile', request);
};

