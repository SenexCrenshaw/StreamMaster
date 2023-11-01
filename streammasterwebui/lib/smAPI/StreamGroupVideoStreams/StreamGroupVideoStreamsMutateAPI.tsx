/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@/lib/iptvApi';
import { invokeHubConnection } from '@/lib/signalr/signalr';

export const SetVideoStreamRanks = async (argument: iptv.SetVideoStreamRanksRequest): Promise<void | null> => {
  await invokeHubConnection<void>('SetVideoStreamRanks', argument);
};
export const SyncVideoStreamToStreamGroupPOST = async (argument: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void | null> => {
  await invokeHubConnection<void>('SyncVideoStreamToStreamGroupPOST', argument);
};
export const SyncVideoStreamToStreamGroupDELETE = async (argument: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void | null> => {
  await invokeHubConnection<void>('SyncVideoStreamToStreamGroupDELETE', argument);
};
export const SetStreamGroupVideoStreamChannelNumbers = async (argument: iptv.SetStreamGroupVideoStreamChannelNumbersRequest): Promise<void | null> => {
  await invokeHubConnection<void>('SetStreamGroupVideoStreamChannelNumbers', argument);
};
