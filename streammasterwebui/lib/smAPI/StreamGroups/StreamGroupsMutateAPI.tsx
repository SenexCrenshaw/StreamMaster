/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const CreateStreamGroup = async (argument: iptv.CreateStreamGroupRequest): Promise<void | null> => {
  await invokeHubConnection<void>('CreateStreamGroup', argument);
};
export const DeleteStreamGroup = async (argument: iptv.DeleteStreamGroupRequest): Promise<void | null> => {
  await invokeHubConnection<void>('DeleteStreamGroup', argument);
};
export const UpdateStreamGroup = async (argument: iptv.UpdateStreamGroupRequest): Promise<void | null> => {
  await invokeHubConnection<void>('UpdateStreamGroup', argument);
};
