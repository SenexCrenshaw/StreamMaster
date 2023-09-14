import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";


export const GetEpgFile = async (arg: iptv.EpgFileDto): Promise<iptv.EpgFileDto> => {
  const data = await hubConnection.invoke('GetEpgFile', arg);
  return data;
};

export const GetPagedEpgFiles = async (arg: iptv.PagedResponseOfEpgFileDto): Promise<iptv.EpgFileDto[]> => {
  const data = await hubConnection.invoke('GetPagedEpgFiles', arg);
  return data;
};

