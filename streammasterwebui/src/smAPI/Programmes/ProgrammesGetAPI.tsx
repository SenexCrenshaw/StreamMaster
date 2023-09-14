import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";


export const GetProgramme = async (arg: iptv.Programme[]): Promise<iptv.Programme[]> => {
  const data = await hubConnection.invoke('GetProgramme', arg);
  return data;
};

export const GetProgrammeChannels = async (arg: iptv.ProgrammeChannel[]): Promise<iptv.ProgrammeChannel[]> => {
  const data = await hubConnection.invoke('GetProgrammeChannels', arg);
  return data;
};

export const GetProgrammeNameSelections = async (arg: iptv.PagedResponseOfProgrammeNameDto): Promise<iptv.ProgrammeNameDto[]> => {
  const data = await hubConnection.invoke('GetProgrammeNameSelections', arg);
  return data;
};

export const GetProgrammes = async (arg: iptv.Programme[]): Promise<iptv.Programme[]> => {
  const data = await hubConnection.invoke('GetProgrammes', arg);
  return data;
};

export const GetProgrammeNames = async (): Promise<void> => {
  await hubConnection.invoke('GetProgrammeNames');
};

export const GetProgrammsSimpleQuery = async (arg: iptv.ProgrammeNameDto[]): Promise<iptv.ProgrammeNameDto[]> => {
  const data = await hubConnection.invoke('GetProgrammsSimpleQuery', arg);
  return data;
};

export const GetProgrammeFromDisplayName = async (arg: iptv.GetProgrammeFromDisplayNameRequest): Promise<iptv.ProgrammeNameDto> => {
  const data = await hubConnection.invoke('GetProgrammeFromDisplayName', arg);
  return data;
};

