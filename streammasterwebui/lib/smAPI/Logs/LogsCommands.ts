import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { GetLogContentsRequest } from '@lib/smAPI/smapiTypes';

export const GetLogContents = async (request: GetLogContentsRequest): Promise<string | undefined> => {
  if ( request === undefined ) {
    return undefined;
  }
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<string>('GetLogContents', request);
};

export const GetLogNames = async (): Promise<string[] | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<string[]>('GetLogNames');
};

