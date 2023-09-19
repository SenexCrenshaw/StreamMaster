import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const SyncStreamGroupChannelGroups = async (arg: iptv.SyncStreamGroupChannelGroupsRequest): Promise<iptv.StreamGroupDto> => {
  if (isDebug) console.log('SyncStreamGroupChannelGroups');
  const data = await hubConnection.invoke('SyncStreamGroupChannelGroups', arg);
  return data;
};

