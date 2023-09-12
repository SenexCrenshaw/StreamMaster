import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetChannelGroupsFromStreamGroup = async (): Promise<iptv.ChannelGroupDto[]> => {
  const data = await hubConnection.invoke('GetChannelGroupsFromStreamGroup');
  return data;
};

export const GetAllChannelGroups = async (): Promise<iptv.ChannelGroupDto[]> => {
  const data = await hubConnection.invoke('GetAllChannelGroups');
  return data;
};

