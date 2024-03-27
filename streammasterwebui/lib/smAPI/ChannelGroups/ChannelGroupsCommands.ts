import {DefaultAPIResponse} from '@lib/apiDefs';
import SignalRService from '@lib/signalr/SignalRService';

import { CreateChannelGroupRequest } from './ChannelGroupsTypes';
export const CreateChannelGroup = async (request: CreateChannelGroupRequest): Promise<DefaultAPIResponse | null> => {
  const signalRService = SignalRService.getInstance();
  return await signalRService.invokeHubCommand<DefaultAPIResponse>('CreateChannelGroup', request);
};

