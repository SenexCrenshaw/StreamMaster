/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@/lib/signalr/signalr';
import type * as iptv from '@/lib/iptvApi';

export const GetProgramme = async (arg: iptv.Programme[]): Promise<iptv.Programme[] | null> => {
  return await invokeHubConnection<iptv.Programme[]>('GetProgramme', arg);
};

export const GetProgrammeChannels = async (arg: iptv.ProgrammeChannel[]): Promise<iptv.ProgrammeChannel[] | null> => {
  return await invokeHubConnection<iptv.ProgrammeChannel[]>('GetProgrammeChannels', arg);
};

export const GetPagedProgrammeNameSelections = async (arg: iptv.PagedResponseOfProgrammeNameDto): Promise<iptv.ProgrammeNameDto[] | null> => {
  return await invokeHubConnection<iptv.ProgrammeNameDto[]>('GetPagedProgrammeNameSelections', arg);
};

export const GetProgrammes = async (arg: iptv.Programme[]): Promise<iptv.Programme[] | null> => {
  return await invokeHubConnection<iptv.Programme[]>('GetProgrammes', arg);
};

export const GetProgrammeNames = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetProgrammeNames');
};

export const GetProgrammsSimpleQuery = async (arg: iptv.ProgrammeNameDto[]): Promise<iptv.ProgrammeNameDto[] | null> => {
  return await invokeHubConnection<iptv.ProgrammeNameDto[]>('GetProgrammsSimpleQuery', arg);
};

export const GetProgrammeFromDisplayName = async (arg: iptv.GetProgrammeFromDisplayNameRequest): Promise<iptv.ProgrammeNameDto | null> => {
  return await invokeHubConnection<iptv.ProgrammeNameDto>('GetProgrammeFromDisplayName', arg);
};
