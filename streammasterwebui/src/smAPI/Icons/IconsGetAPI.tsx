import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";
import { type StringArg } from "../../components/selectors/BaseSelector";


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

