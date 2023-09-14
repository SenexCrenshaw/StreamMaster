import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";
import { type StringArg } from "../../components/selectors/BaseSelector";


export const GetIcon = async (arg: iptv.IconFileDto): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIcon', arg);
  return data;
};

export const GetIconFromSource = async (arg: StringArg): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIconFromSource', arg);
  return data;
};

export const GetIcons = async (arg: iptv.PagedResponseOfIconFileDto): Promise<iptv.IconFileDto[]> => {
  const data = await hubConnection.invoke('GetIcons', arg);
  return data;
};

export const GetIconsSimpleQuery = async (arg: iptv.IconFileDto[]): Promise<iptv.IconFileDto[]> => {
  const data = await hubConnection.invoke('GetIconsSimpleQuery', arg);
  return data;
};

