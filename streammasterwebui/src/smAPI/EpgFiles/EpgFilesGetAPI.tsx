import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";


export const GetEpgFile = async (arg: iptv.EpgFileDto): Promise<iptv.EpgFileDto> => {
  if (isDebug) console.log('GetEpgFile');
  const data = await hubConnection.invoke('GetEpgFile', arg);
  return data;
};

export const GetPagedEpgFiles = async (arg: iptv.PagedResponseOfEpgFileDto): Promise<iptv.EpgFileDto[]> => {
  if (isDebug) console.log('GetPagedEpgFiles');
  const data = await hubConnection.invoke('GetPagedEpgFiles', arg);
  return data;
};

