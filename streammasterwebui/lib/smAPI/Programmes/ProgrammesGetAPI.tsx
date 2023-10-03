/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetProgramme = async (arg: iptv.Programme[]): Promise<iptv.Programme[]> => {
  if (isDebug) console.log('GetProgramme');
  const data = await hubConnection.invoke('GetProgramme', arg);
  return data;
};

export const GetProgrammeChannels = async (arg: iptv.ProgrammeChannel[]): Promise<iptv.ProgrammeChannel[]> => {
  if (isDebug) console.log('GetProgrammeChannels');
  const data = await hubConnection.invoke('GetProgrammeChannels', arg);
  return data;
};

export const GetPagedProgrammeNameSelections = async (arg: iptv.PagedResponseOfProgrammeNameDto): Promise<iptv.ProgrammeNameDto[]> => {
  if (isDebug) console.log('GetPagedProgrammeNameSelections');
  const data = await hubConnection.invoke('GetPagedProgrammeNameSelections', arg);
  return data;
};

export const GetProgrammes = async (arg: iptv.Programme[]): Promise<iptv.Programme[]> => {
  if (isDebug) console.log('GetProgrammes');
  const data = await hubConnection.invoke('GetProgrammes', arg);
  return data;
};

export const GetProgrammeNames = async (): Promise<void> => {
  if (isDebug) console.log('GetProgrammeNames');
  await hubConnection.invoke('GetProgrammeNames');
};

export const GetProgrammsSimpleQuery = async (arg: iptv.ProgrammeNameDto[]): Promise<iptv.ProgrammeNameDto[]> => {
  if (isDebug) console.log('GetProgrammsSimpleQuery');
  const data = await hubConnection.invoke('GetProgrammsSimpleQuery', arg);
  return data;
};

export const GetProgrammeFromDisplayName = async (arg: iptv.GetProgrammeFromDisplayNameRequest): Promise<iptv.ProgrammeNameDto> => {
  if (isDebug) console.log('GetProgrammeFromDisplayName');
  const data = await hubConnection.invoke('GetProgrammeFromDisplayName', arg);
  return data;
};

