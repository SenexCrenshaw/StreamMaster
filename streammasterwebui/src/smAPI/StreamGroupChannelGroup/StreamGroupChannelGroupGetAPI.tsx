import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";


export const GetChannelGroupsFromStreamGroup = async (arg: iptv.ChannelGroupDto[]): Promise<iptv.ChannelGroupDto[]> => {
  if (isDebug) console.log('GetChannelGroupsFromStreamGroup');
  const data = await hubConnection.invoke('GetChannelGroupsFromStreamGroup', arg);
  return data;
};

