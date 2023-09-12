import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetM3UFile = async (): Promise<iptv.M3UFileDto> => {
  const data = await hubConnection.invoke('GetM3UFile');
  return data;
};

export const GetM3UFiles = async (): Promise<iptv.PagedResponseOfM3UFileDto> => {
  const data = await hubConnection.invoke('GetM3UFiles');
  return data;
};

export const GetM3UFileNames = async (): Promise<void> => {
  await hubConnection.invoke('GetM3UFileNames');
};

