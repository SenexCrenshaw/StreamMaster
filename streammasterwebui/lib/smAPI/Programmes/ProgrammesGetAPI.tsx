/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@/lib/iptvApi';
import { invokeHubConnection } from '@/lib/signalr/signalr';

export const GetProgramme = async (argument: iptv.Programme[]): Promise<iptv.Programme[] | null> =>
  invokeHubConnection<iptv.Programme[]>('GetProgramme', argument);
export const GetProgrammeChannels = async (argument: iptv.ProgrammeChannel[]): Promise<iptv.ProgrammeChannel[] | null> =>
  invokeHubConnection<iptv.ProgrammeChannel[]>('GetProgrammeChannels', argument);
export const GetPagedProgrammeNameSelections = async (argument: iptv.PagedResponseOfProgrammeNameDto): Promise<iptv.ProgrammeNameDto[] | null> =>
  invokeHubConnection<iptv.ProgrammeNameDto[]>('GetPagedProgrammeNameSelections', argument);
export const GetProgrammes = async (argument: iptv.Programme[]): Promise<iptv.Programme[] | null> =>
  invokeHubConnection<iptv.Programme[]>('GetProgrammes', argument);
export const GetProgrammeNames = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetProgrammeNames');
};
export const GetProgrammsSimpleQuery = async (argument: iptv.ProgrammeNameDto[]): Promise<iptv.ProgrammeNameDto[] | null> =>
  invokeHubConnection<iptv.ProgrammeNameDto[]>('GetProgrammsSimpleQuery', argument);
export const GetProgrammeFromDisplayName = async (argument: iptv.GetProgrammeFromDisplayNameRequest): Promise<iptv.ProgrammeNameDto | null> =>
  invokeHubConnection<iptv.ProgrammeNameDto>('GetProgrammeFromDisplayName', argument);
