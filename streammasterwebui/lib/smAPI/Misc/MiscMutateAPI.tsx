/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@lib/signalr/signalr';

export const BuildIconsCacheFromVideoStreams = async (): Promise<void | null> => {
  await invokeHubConnection<void>('BuildIconsCacheFromVideoStreams');
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void | null> => {
  await invokeHubConnection<void>('BuildProgIconsCacheFromEpgsRequest');
};
