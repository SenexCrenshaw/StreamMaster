import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetM3UFile = async (arg: iptv.M3UFileDto): Promise<iptv.M3UFileDto> => {
  if (isDebug) console.log('GetM3UFile');
  const data = await hubConnection.invoke('GetM3UFile', arg);
  return data;
};

export const GetPagedM3UFiles = async (arg: iptv.PagedResponseOfM3UFileDto): Promise<iptv.M3UFileDto[]> => {
  if (isDebug) console.log('GetPagedM3UFiles');
  const data = await hubConnection.invoke('GetPagedM3UFiles', arg);
  return data;
};

export const GetM3UFileNames = async (): Promise<void> => {
  if (isDebug) console.log('GetM3UFileNames');
  await hubConnection.invoke('GetM3UFileNames');
};

