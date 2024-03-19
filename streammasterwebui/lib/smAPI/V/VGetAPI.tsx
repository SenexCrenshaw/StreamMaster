/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetVideoStreamStreamGET = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamGET', argument);
};
export const GetVideoStreamStreamGET2 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamGET2', argument);
};
export const GetStreamGroupM3U = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupM3U', argument);
};
export const GetStreamGroupEpg = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupEpg', argument);
};
