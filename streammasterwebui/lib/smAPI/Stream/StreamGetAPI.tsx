/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@lib/common/dataTypes';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetM3U8GET = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetM3U8GET', argument);
};
export const GetVideoStreamGET = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamGET', argument);
};
