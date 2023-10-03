/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


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

