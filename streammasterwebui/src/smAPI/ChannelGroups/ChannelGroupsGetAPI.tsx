import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetChannelGroup = async (arg: iptv.ChannelGroupDto): Promise<iptv.ChannelGroupDto> => {
  const data = await hubConnection.invoke('GetChannelGroup', arg);
  return data;
};

export const GetChannelGroupIdNames = async (arg: iptv.ChannelGroupIdName[]): Promise<iptv.ChannelGroupIdName[]> => {
  const data = await hubConnection.invoke('GetChannelGroupIdNames', arg);
  return data;
};

export const GetChannelGroups = async (arg: iptv.PagedResponseOfChannelGroupDto): Promise<iptv.ChannelGroupDto[]> => {
  const data = await hubConnection.invoke('GetChannelGroups', arg);
  return data;
};

export const GetChannelGroupNames = async (): Promise<void> => {
  await hubConnection.invoke('GetChannelGroupNames');
};

