import { isSkipToken } from '@lib/common/isSkipToken';
import SignalRService from '@lib/signalr/SignalRService';
import { APIResponse,CancelAllChannelsRequest,CancelChannelRequest,CancelClientRequest,MoveToNextStreamRequest } from '@lib/smAPI/smapiTypes';

export const CancelAllChannels = async (): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CancelAllChannels');
};

export const CancelChannel = async (request: CancelChannelRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CancelChannel', request);
};

export const CancelClient = async (request: CancelClientRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('CancelClient', request);
};

export const MoveToNextStream = async (request: MoveToNextStreamRequest): Promise<APIResponse | undefined> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<APIResponse>('MoveToNextStream', request);
};

