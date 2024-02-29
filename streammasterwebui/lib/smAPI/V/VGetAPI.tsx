/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@lib/common/dataTypes';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetVideoStreamStreamGET = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET', argument);
};
export const GetVideoStreamStreamGET2 = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET2', argument);
};
export const GetStreamGroupM3U = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupM3U', argument);
};
export const GetStreamGroupEpg = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupEpg', argument);
};
