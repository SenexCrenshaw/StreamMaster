/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubCommand } from '@lib/signalr/signalr';

export const BuildIconsCacheFromVideoStreams = async (): Promise<void | null> => {
  await invokeHubCommand<void>('BuildIconsCacheFromVideoStreams');
};
export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void | null> => {
  await invokeHubCommand<void>('BuildProgIconsCacheFromEpgsRequest');
};
