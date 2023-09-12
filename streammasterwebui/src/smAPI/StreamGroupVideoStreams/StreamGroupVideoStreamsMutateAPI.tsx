import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const SetVideoStreamRanks = async (arg: iptv.SetVideoStreamRanksRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamRanks', arg);
};

export const SyncVideoStreamToStreamGroupPOST = async (arg: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('SyncVideoStreamToStreamGroupPOST', arg);
};

export const SyncVideoStreamToStreamGroupDELETE = async (arg: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void> => {
  await hubConnection.invoke('SyncVideoStreamToStreamGroupDELETE', arg);
};

