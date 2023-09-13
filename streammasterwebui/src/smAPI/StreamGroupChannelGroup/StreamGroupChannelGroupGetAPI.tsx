import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetChannelGroupsFromStreamGroup = async (arg: iptv.ChannelGroupDto[]): Promise<iptv.ChannelGroupDto[]> => {
  const data = await hubConnection.invoke('GetChannelGroupsFromStreamGroup', arg);
  return data;
};

export const GetAllChannelGroups = async (arg: iptv.ChannelGroupDto[]): Promise<iptv.ChannelGroupDto[]> => {
  const data = await hubConnection.invoke('GetAllChannelGroups', arg);
  return data;
};

