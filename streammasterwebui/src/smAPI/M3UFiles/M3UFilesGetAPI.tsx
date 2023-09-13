import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetM3UFile = async (arg: iptv.M3UFileDto): Promise<iptv.M3UFileDto> => {
  const data = await hubConnection.invoke('GetM3UFile', arg);
  return data;
};

export const GetM3UFiles = async (arg: iptv.PagedResponseOfM3UFileDto): Promise<iptv.M3UFileDto[]> => {
  const data = await hubConnection.invoke('GetM3UFiles', arg);
  return data;
};

export const GetM3UFileNames = async (): Promise<void> => {
  await hubConnection.invoke('GetM3UFileNames');
};

