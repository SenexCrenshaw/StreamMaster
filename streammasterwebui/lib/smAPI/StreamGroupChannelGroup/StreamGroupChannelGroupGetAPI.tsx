import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetChannelGroupsFromStreamGroup = async (arg: iptv.ChannelGroupDto[]): Promise<iptv.ChannelGroupDto[]> => {
  if (isDebug) console.log('GetChannelGroupsFromStreamGroup');
  const data = await hubConnection.invoke('GetChannelGroupsFromStreamGroup', arg);
  return data;
};

