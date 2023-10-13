/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@/lib/signalr/signalr';
import type * as iptv from '@/lib/iptvApi';

export const GetFile = async (arg: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetFile', arg);
};
