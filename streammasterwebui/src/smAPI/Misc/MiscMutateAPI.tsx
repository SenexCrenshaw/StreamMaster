import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const BuildIconsCacheFromVideoStreams = async (): Promise<void> => {
  await hubConnection.invoke('BuildIconsCacheFromVideoStreams');
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void> => {
  await hubConnection.invoke('BuildProgIconsCacheFromEpgsRequest');
};

