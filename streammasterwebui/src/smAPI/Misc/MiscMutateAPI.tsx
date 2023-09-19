import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const BuildIconsCacheFromVideoStreams = async (): Promise<void> => {
  if (isDebug) console.log('BuildIconsCacheFromVideoStreams');
  await hubConnection.invoke('BuildIconsCacheFromVideoStreams');
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void> => {
  if (isDebug) console.log('BuildProgIconsCacheFromEpgsRequest');
  await hubConnection.invoke('BuildProgIconsCacheFromEpgsRequest');
};

