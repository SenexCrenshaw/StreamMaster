import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetProgramme = async (): Promise<iptv.Programme[]> => {
  const data = await hubConnection.invoke('GetProgramme');
  return data;
};

export const GetProgrammeChannels = async (): Promise<iptv.ProgrammeChannel[]> => {
  const data = await hubConnection.invoke('GetProgrammeChannels');
  return data;
};

export const GetProgrammeNameSelections = async (): Promise<iptv.PagedResponseOfProgrammeNameDto> => {
  const data = await hubConnection.invoke('GetProgrammeNameSelections');
  return data;
};

export const GetProgrammes = async (): Promise<iptv.Programme[]> => {
  const data = await hubConnection.invoke('GetProgrammes');
  return data;
};

export const GetProgrammeNames = async (): Promise<void> => {
  await hubConnection.invoke('GetProgrammeNames');
};

export const GetProgrammsSimpleQuery = async (): Promise<iptv.ProgrammeNameDto[]> => {
  const data = await hubConnection.invoke('GetProgrammsSimpleQuery');
  return data;
};

export const GetProgrammeFromDisplayName = async (): Promise<iptv.ProgrammeNameDto> => {
  const data = await hubConnection.invoke('GetProgrammeFromDisplayName');
  return data;
};

