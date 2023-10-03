/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const BuildIconsCacheFromVideoStreams = async (): Promise<void | null> => {
    await invokeHubConnection<void> ('BuildIconsCacheFromVideoStreams');
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void | null> => {
    await invokeHubConnection<void> ('BuildProgIconsCacheFromEpgsRequest');
};

