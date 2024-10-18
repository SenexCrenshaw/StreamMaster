import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { V,GetVsRequest } from '@lib/smAPI/smapiTypes';

export const GetVs = async (request: GetVsRequest): Promise<V[] | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<V[]>('GetVs', request);
};

