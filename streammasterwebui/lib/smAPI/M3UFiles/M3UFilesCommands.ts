import {CreateM3UFileRequest,DefaultAPIResponse,DeleteM3UFileRequest,ProcessM3UFileRequest,RefreshM3UFileRequest} from '@lib/smAPI/smapiTypes';
import {APIResponse,PagedResponse,QueryStringParameters,M3UFileDto} from '@lib/smAPI/smapiTypes';
import SignalRService from '@lib/signalr/SignalRService';

export const CreateM3UFile = async (request: CreateM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateM3UFile', request);
};

export const DeleteM3UFile = async (request: DeleteM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('DeleteM3UFile', request);
};

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

export const ProcessM3UFile = async (request: ProcessM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('ProcessM3UFile', request);
};

export const RefreshM3UFile = async (request: RefreshM3UFileRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('RefreshM3UFile', request);
};

