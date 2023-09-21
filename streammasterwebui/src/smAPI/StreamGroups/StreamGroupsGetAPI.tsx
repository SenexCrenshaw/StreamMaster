import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";


export const GetStreamGroup = async (arg: iptv.StreamGroupDto): Promise<iptv.StreamGroupDto> => {
  if (isDebug) console.log('GetStreamGroup');
  const data = await hubConnection.invoke('GetStreamGroup', arg);
  return data;
};

export const GetStreamGroupCapability = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupCapability');
  await hubConnection.invoke('GetStreamGroupCapability', arg);
};

export const GetStreamGroupCapability2 = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupCapability2');
  await hubConnection.invoke('GetStreamGroupCapability2', arg);
};

export const GetStreamGroupCapability3 = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupCapability3');
  await hubConnection.invoke('GetStreamGroupCapability3', arg);
};

export const GetStreamGroupDiscover = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupDiscover');
  await hubConnection.invoke('GetStreamGroupDiscover', arg);
};

export const GetStreamGroupEpg = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupEpg');
  await hubConnection.invoke('GetStreamGroupEpg', arg);
};

export const GetStreamGroupEpgForGuide = async (arg: iptv.EpgGuide): Promise<iptv.EpgGuide> => {
  if (isDebug) console.log('GetStreamGroupEpgForGuide');
  const data = await hubConnection.invoke('GetStreamGroupEpgForGuide', arg);
  return data;
};

export const GetStreamGroupLineUp = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupLineUp');
  await hubConnection.invoke('GetStreamGroupLineUp', arg);
};

export const GetStreamGroupLineUpStatus = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupLineUpStatus');
  await hubConnection.invoke('GetStreamGroupLineUpStatus', arg);
};

export const GetStreamGroupM3U = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetStreamGroupM3U');
  await hubConnection.invoke('GetStreamGroupM3U', arg);
};

export const GetPagedStreamGroups = async (arg: iptv.PagedResponseOfStreamGroupDto): Promise<iptv.StreamGroupDto[]> => {
  if (isDebug) console.log('GetPagedStreamGroups');
  const data = await hubConnection.invoke('GetPagedStreamGroups', arg);
  return data;
};

