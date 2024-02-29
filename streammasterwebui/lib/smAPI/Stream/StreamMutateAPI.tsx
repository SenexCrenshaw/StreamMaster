/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetM3U8HEAD = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetM3U8HEAD', argument);
};
export const GetVideoStreamHEAD = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamHEAD', argument);
};
