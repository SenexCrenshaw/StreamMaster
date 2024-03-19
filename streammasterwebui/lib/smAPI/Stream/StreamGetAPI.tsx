/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetM3U8GET = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetM3U8GET', argument);
};
export const GetVideoStreamGET = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamGET', argument);
};
