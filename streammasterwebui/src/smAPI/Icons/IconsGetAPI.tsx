import { hubConnection } from "../../app/signalr";
import { type StringArg } from "../../components/selectors/BaseSelector";
import type * as iptv from "../../store/iptvApi";

export const GetIcon = async (arg: iptv.IconFileDto): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIcon', arg);
  return data;
};

export const GetIconFromSource = async (arg: iptv.GetProgrammeFromDisplayNameRequest): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIconFromSource', arg);
  return data;
};

export const GetIcons = async (arg: StringArg): Promise<iptv.IconFileDto[]> => {
  const data = await hubConnection.invoke('GetIcons', arg);
  return data;
};

export const GetIconsSimpleQuery = async (arg: iptv.IconFileDto[]): Promise<iptv.IconFileDto[]> => {
  const data = await hubConnection.invoke('GetIconsSimpleQuery', arg);
  return data;
};

