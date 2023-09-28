import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const BuildIconsCacheFromVideoStreams = async (): Promise<void> => {
  if (isDebug) console.log('BuildIconsCacheFromVideoStreams');
  await hubConnection.invoke('BuildIconsCacheFromVideoStreams');
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void> => {
  if (isDebug) console.log('BuildProgIconsCacheFromEpgsRequest');
  await hubConnection.invoke('BuildProgIconsCacheFromEpgsRequest');
};

