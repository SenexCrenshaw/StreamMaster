/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetCountries = async (argument: iptv.Countries): Promise<iptv.Countries | null> =>
  invokeHubConnection<iptv.Countries>('GetCountries', argument);
export const GetHeadends = async (argument: iptv.HeadendDto[]): Promise<iptv.HeadendDto[] | null> =>
  invokeHubConnection<iptv.HeadendDto[]>('GetHeadends', argument);
export const GetLineup = async (argument: iptv.LineupResult): Promise<iptv.LineupResult | null> =>
  invokeHubConnection<iptv.LineupResult>('GetLineup', argument);
export const GetLineupPreviews = async (argument: iptv.LineupPreview[]): Promise<iptv.LineupPreview[] | null> =>
  invokeHubConnection<iptv.LineupPreview[]>('GetLineupPreviews', argument);
export const GetLineups = async (argument: iptv.Lineup[]): Promise<iptv.Lineup[] | null> =>
  invokeHubConnection<iptv.Lineup[]>('GetLineups', argument);
export const GetSDPrograms = async (argument: iptv.SdProgram): Promise<iptv.SDProgram[] | null> =>
  invokeHubConnection<iptv.SDProgram[]>('GetSDPrograms', argument);
export const GetSchedules = async (argument: iptv.Schedule[]): Promise<iptv.Schedule[] | null> =>
  invokeHubConnection<iptv.Schedule[]>('GetSchedules', argument);
export const GetSelectedStationIds = async (argument: iptv.StationIdLineup[]): Promise<iptv.StationIdLineup[] | null> =>
  invokeHubConnection<iptv.StationIdLineup[]>('GetSelectedStationIds', argument);
export const GetStationPreviews = async (argument: iptv.StationPreview[]): Promise<iptv.StationPreview[] | null> =>
  invokeHubConnection<iptv.StationPreview[]>('GetStationPreviews', argument);
export const GetStations = async (argument: iptv.Station[]): Promise<iptv.Station[] | null> =>
  invokeHubConnection<iptv.Station[]>('GetStations', argument);
export const GetStatus = async (argument: iptv.SdStatus): Promise<iptv.SdStatus | null> =>
  invokeHubConnection<iptv.SdStatus>('GetStatus', argument);
export const GetEpg = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetEpg');
};
export const GetLineupNames = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetLineupNames');
};
