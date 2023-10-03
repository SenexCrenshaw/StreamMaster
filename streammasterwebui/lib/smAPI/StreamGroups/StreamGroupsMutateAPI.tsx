/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const CreateStreamGroup = async (arg: iptv.CreateStreamGroupRequest): Promise<void> => {
  if (isDebug) console.log('CreateStreamGroup');
  await hubConnection.invoke('CreateStreamGroup', arg);
};

export const DeleteStreamGroup = async (arg: iptv.DeleteStreamGroupRequest): Promise<void> => {
  if (isDebug) console.log('DeleteStreamGroup');
  await hubConnection.invoke('DeleteStreamGroup', arg);
};

export const UpdateStreamGroup = async (arg: iptv.UpdateStreamGroupRequest): Promise<void> => {
  if (isDebug) console.log('UpdateStreamGroup');
  await hubConnection.invoke('UpdateStreamGroup', arg);
};

