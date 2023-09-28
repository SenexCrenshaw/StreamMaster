import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const SyncStreamGroupChannelGroups = async (arg: iptv.SyncStreamGroupChannelGroupsRequest): Promise<iptv.StreamGroupDto> => {
  if (isDebug) console.log('SyncStreamGroupChannelGroups');
  const data = await hubConnection.invoke('SyncStreamGroupChannelGroups', arg);
  return data;
};

