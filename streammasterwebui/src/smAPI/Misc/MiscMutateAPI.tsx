import { hubConnection } from "../../app/signalr";

export const BuildIconsCacheFromVideoStreams = async (): Promise<void> => {
  await hubConnection.invoke('BuildIconsCacheFromVideoStreams');
};

export const BuildProgIconsCacheFromEpgsRequest = async (): Promise<void> => {
  await hubConnection.invoke('BuildProgIconsCacheFromEpgsRequest');
};

