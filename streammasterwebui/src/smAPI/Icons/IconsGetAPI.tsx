import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetIcon = async (): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIcon');
  return data;
};

export const GetIconFromSource = async (): Promise<iptv.IconFileDto> => {
  const data = await hubConnection.invoke('GetIconFromSource');
  return data;
};

export const GetIcons = async (): Promise<iptv.PagedResponseOfIconFileDto> => {
  const data = await hubConnection.invoke('GetIcons');
  return data;
};

export const GetIconsSimpleQuery = async (): Promise<iptv.IconFileDto[]> => {
  const data = await hubConnection.invoke('GetIconsSimpleQuery');
  return data;
};

