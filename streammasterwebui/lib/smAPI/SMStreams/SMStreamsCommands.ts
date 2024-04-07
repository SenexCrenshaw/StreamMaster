import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,ToggleSMStreamVisibleByIdRequest,SMStreamDto,GetPagedSMStreamsRequest,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

export const GetPagedSMStreams = async (parameters: QueryStringParameters): Promise<PagedResponse<SMStreamDto> | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<PagedResponse<SMStreamDto>>('GetPagedSMStreams', parameters);
};

export const ToggleSMStreamVisibleById = async (request: ToggleSMStreamVisibleByIdRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ToggleSMStreamVisibleById', request);
};

