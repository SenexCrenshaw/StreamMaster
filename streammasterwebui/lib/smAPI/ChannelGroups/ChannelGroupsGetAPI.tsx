/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetChannelGroup = async (arg: iptv.ChannelGroupDto): Promise<iptv.ChannelGroupDto> => {
  if (isDebug) console.log('GetChannelGroup');
  const data = await hubConnection.invoke('GetChannelGroup', arg);
  return data;
};

export const GetChannelGroupIdNames = async (arg: iptv.ChannelGroupIdName[]): Promise<iptv.ChannelGroupIdName[]> => {
  if (isDebug) console.log('GetChannelGroupIdNames');
  const data = await hubConnection.invoke('GetChannelGroupIdNames', arg);
  return data;
};

export const GetPagedChannelGroups = async (arg: iptv.PagedResponseOfChannelGroupDto): Promise<iptv.ChannelGroupDto[]> => {
  if (isDebug) console.log('GetPagedChannelGroups');
  const data = await hubConnection.invoke('GetPagedChannelGroups', arg);
  return data;
};

export const GetChannelGroupNames = async (): Promise<void> => {
  if (isDebug) console.log('GetChannelGroupNames');
  await hubConnection.invoke('GetChannelGroupNames');
};

export const GetChannelGroupsForStreamGroup = async (arg: iptv.GetChannelGroupsForStreamGroupRequest): Promise<iptv.ChannelGroupDto[]> => {
  if (isDebug) console.log('GetChannelGroupsForStreamGroup');
  const data = await hubConnection.invoke('GetChannelGroupsForStreamGroup', arg);
  return data;
};

