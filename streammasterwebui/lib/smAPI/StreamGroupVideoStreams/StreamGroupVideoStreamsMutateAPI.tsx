/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const SetVideoStreamRanks = async (argument: iptv.SetVideoStreamRanksRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetVideoStreamRanks', argument);
};
export const SyncVideoStreamToStreamGroupPOST = async (argument: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SyncVideoStreamToStreamGroupPOST', argument);
};
export const SyncVideoStreamToStreamGroupDELETE = async (argument: iptv.SyncVideoStreamToStreamGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SyncVideoStreamToStreamGroupDELETE', argument);
};
export const SetStreamGroupVideoStreamChannelNumbers = async (argument: iptv.SetStreamGroupVideoStreamChannelNumbersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetStreamGroupVideoStreamChannelNumbers', argument);
};
