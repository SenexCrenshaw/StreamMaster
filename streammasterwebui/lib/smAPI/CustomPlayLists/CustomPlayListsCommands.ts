import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,ScanForCustomPlayListsRequest,CustomPlayList,GetCustomPlayListRequest } from '@lib/smAPI/smapiTypes';

export const GetCustomPlayList = async (request: GetCustomPlayListRequest): Promise<CustomPlayList | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<CustomPlayList>('GetCustomPlayList', request);
};

export const GetCustomPlayLists = async (): Promise<CustomPlayList[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<CustomPlayList[]>('GetCustomPlayLists');
};

export const ScanForCustomPlayLists = async (): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ScanForCustomPlayLists');
};

