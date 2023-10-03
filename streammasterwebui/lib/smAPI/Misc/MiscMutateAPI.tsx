import { isDebug } from '@/lib/settings';
import { hubConnection } from '@/lib/signalr/signalr';

export const BuildIconsCacheFromVideoStreams = async (): Promise<void> => {
  if (isDebug) console.log('BuildIconsCacheFromVideoStreams');
  await hubConnection.invoke('BuildIconsCacheFromVideoStreams');
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void> => {
  if (isDebug) console.log('BuildProgIconsCacheFromEpgsRequest');
  await hubConnection.invoke('BuildProgIconsCacheFromEpgsRequest');
};

