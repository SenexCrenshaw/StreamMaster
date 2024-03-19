/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetVideoStreamStreamHEAD = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamHEAD', argument);
};
export const GetVideoStreamStreamHEAD2 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamHEAD2', argument);
};
