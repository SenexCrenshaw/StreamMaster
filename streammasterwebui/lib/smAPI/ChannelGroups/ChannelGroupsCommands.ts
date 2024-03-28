import {DefaultAPIResponse,CreateChannelGroupRequest} from '@lib/smAPI/smapiTypes';
import SignalRService from '@lib/signalr/SignalRService';

export const CreateChannelGroup = async (request: CreateChannelGroupRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateChannelGroup', request);
};

