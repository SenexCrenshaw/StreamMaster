import {DefaultAPIResponse} from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

import { CreateChannelGroupRequest } from './ChannelGroupsTypes';
export const CreateChannelGroup = async (request: CreateChannelGroupRequest): Promise<DefaultAPIResponse | null> => {
  return await invokeHubCommand<DefaultAPIResponse>('CreateChannelGroup', request);
};

