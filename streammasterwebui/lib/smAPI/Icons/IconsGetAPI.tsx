/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';
import { type StringArg } from '@/src/components/selectors/BaseSelector';


export const GetIcon = async (arg: iptv.IconFileDto): Promise<iptv.IconFileDto> => {
  if (isDebug) console.log('GetIcon');
  const data = await hubConnection.invoke('GetIcon', arg);
  return data;
};

export const GetIconFromSource = async (arg: StringArg): Promise<iptv.IconFileDto> => {
  if (isDebug) console.log('GetIconFromSource');
  const data = await hubConnection.invoke('GetIconFromSource', arg);
  return data;
};

export const GetPagedIcons = async (arg: iptv.PagedResponseOfIconFileDto): Promise<iptv.IconFileDto[]> => {
  if (isDebug) console.log('GetPagedIcons');
  const data = await hubConnection.invoke('GetPagedIcons', arg);
  return data;
};

export const GetIconsSimpleQuery = async (arg: iptv.IconFileDto[]): Promise<iptv.IconFileDto[]> => {
  if (isDebug) console.log('GetIconsSimpleQuery');
  const data = await hubConnection.invoke('GetIconsSimpleQuery', arg);
  return data;
};

