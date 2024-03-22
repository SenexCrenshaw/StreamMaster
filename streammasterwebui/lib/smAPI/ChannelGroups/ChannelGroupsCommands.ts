import {APIResponse, PagedResponse, ChannelGroupDto, mainEntityName } from '@lib/apiDefs';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const CreateChannelGroupRequest = async (GroupName: string, IsReadOnly: boolean): Promise<any | null> => {
  return await invokeHubCommand<ChannelGroupDto>('CreateChannelGroupRequest', GroupName, IsReadOnly);
};

