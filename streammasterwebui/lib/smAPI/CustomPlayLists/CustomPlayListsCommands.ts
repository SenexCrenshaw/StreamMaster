import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,ScanForCustomPlayListsRequest } from '@lib/smAPI/smapiTypes';

export const ScanForCustomPlayLists = async (): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('ScanForCustomPlayLists');
};

