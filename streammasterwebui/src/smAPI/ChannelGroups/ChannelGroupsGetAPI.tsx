import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";


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

