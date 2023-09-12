import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const SyncStreamGroupChannelGroups = async (arg: iptv.SyncStreamGroupChannelGroupsRequest): Promise<iptv.StreamGroupDto> => {
  const data = await hubConnection.invoke('SyncStreamGroupChannelGroups', arg);
  return data;
};

