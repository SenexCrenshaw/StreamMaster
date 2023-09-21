import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const SetVideoStreamRanks = async (arg: iptv.SetVideoStreamRanksRequest): Promise<void> => {
  if (isDebug) console.log('SetVideoStreamRanks');
  await hubConnection.invoke('SetVideoStreamRanks', arg);
};

export const SyncVideoStreamToStreamGroupPOST = async (arg: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void> => {
  if (isDebug) console.log('SyncVideoStreamToStreamGroupPOST');
  await hubConnection.invoke('SyncVideoStreamToStreamGroupPOST', arg);
};

export const SyncVideoStreamToStreamGroupDELETE = async (arg: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void> => {
  if (isDebug) console.log('SyncVideoStreamToStreamGroupDELETE');
  await hubConnection.invoke('SyncVideoStreamToStreamGroupDELETE', arg);
};

