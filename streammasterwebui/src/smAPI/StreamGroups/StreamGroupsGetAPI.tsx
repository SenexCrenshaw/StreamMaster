import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetStreamGroup = async (): Promise<iptv.StreamGroupDto> => {
  const data = await hubConnection.invoke('GetStreamGroup');
  return data;
};

export const GetStreamGroupCapability = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupCapability');
};

export const GetStreamGroupCapability2 = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupCapability2');
};

export const GetStreamGroupCapability3 = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupCapability3');
};

export const GetStreamGroupDiscover = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupDiscover');
};

export const GetStreamGroupEpg = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupEpg');
};

export const GetStreamGroupEpgForGuide = async (): Promise<iptv.EpgGuide> => {
  const data = await hubConnection.invoke('GetStreamGroupEpgForGuide');
  return data;
};

export const GetStreamGroupLineUp = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupLineUp');
};

export const GetStreamGroupLineUpStatus = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupLineUpStatus');
};

export const GetStreamGroupM3U = async (): Promise<void> => {
  await hubConnection.invoke('GetStreamGroupM3U');
};

export const GetStreamGroups = async (): Promise<iptv.PagedResponseOfStreamGroupDto> => {
  const data = await hubConnection.invoke('GetStreamGroups');
  return data;
};

