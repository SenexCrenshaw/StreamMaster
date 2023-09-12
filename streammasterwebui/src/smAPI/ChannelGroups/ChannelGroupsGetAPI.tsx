import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetChannelGroup = async (): Promise<iptv.ChannelGroupDto> => {
  const data = await hubConnection.invoke('GetChannelGroup');
  return data;
};

export const GetChannelGroupIdNames = async (): Promise<iptv.ChannelGroupIdName[]> => {
  const data = await hubConnection.invoke('GetChannelGroupIdNames');
  return data;
};

export const GetChannelGroups = async (): Promise<iptv.PagedResponseOfChannelGroupDto> => {
  const data = await hubConnection.invoke('GetChannelGroups');
  return data;
};

export const GetChannelGroupNames = async (): Promise<void> => {
  await hubConnection.invoke('GetChannelGroupNames');
};

