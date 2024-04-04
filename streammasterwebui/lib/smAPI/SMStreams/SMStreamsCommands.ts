import SignalRService from '@lib/signalr/SignalRService';
import { DefaultAPIResponse,ToggleSMStreamVisibleByIdRequest,SMStreamDto,APIResponse,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedSMStreams = async (parameters: QueryStringParameters): Promise<PagedResponse<SMStreamDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse<SMStreamDto>>('GetPagedSMStreams', parameters)
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

export const ToggleSMStreamVisibleById = async (request: ToggleSMStreamVisibleByIdRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('ToggleSMStreamVisibleById', request);
};

