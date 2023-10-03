/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetFile = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetFile');
  await hubConnection.invoke('GetFile', arg);
};

