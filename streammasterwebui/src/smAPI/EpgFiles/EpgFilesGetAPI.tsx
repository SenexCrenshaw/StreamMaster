import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetEpgFile = async (): Promise<iptv.EpgFileDto> => {
  const data = await hubConnection.invoke('GetEpgFile');
  return data;
};

export const GetEpgFiles = async (): Promise<iptv.PagedResponseOfEpgFileDto> => {
  const data = await hubConnection.invoke('GetEpgFiles');
  return data;
};

