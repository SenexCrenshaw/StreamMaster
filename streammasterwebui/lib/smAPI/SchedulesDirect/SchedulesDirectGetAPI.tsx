/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@/lib/iptvApi';
import { invokeHubConnection } from '@/lib/signalr/signalr';

export const GetCountries = async (argument: iptv.ICountries): Promise<iptv.ICountries | null> =>
  invokeHubConnection<iptv.ICountries>('GetCountries', argument);
export const GetHeadends = async (argument: iptv.HeadendDto[]): Promise<iptv.HeadendDto[] | null> =>
  invokeHubConnection<iptv.HeadendDto[]>('GetHeadends', argument);
export const GetLineup = async (argument: iptv.ILineUpResult): Promise<iptv.ILineUpResult | null> =>
  invokeHubConnection<iptv.ILineUpResult>('GetLineup', argument);
export const GetLineupPreviews = async (argument: iptv.ILineUpPreview[]): Promise<iptv.ILineUpPreview[] | null> =>
  invokeHubConnection<iptv.ILineUpPreview[]>('GetLineupPreviews', argument);
export const GetLineups = async (argument: iptv.ILineUpsResult): Promise<iptv.ILineUpsResult | null> =>
  invokeHubConnection<iptv.ILineUpsResult>('GetLineups', argument);
export const GetSDPrograms = async (argument: SdProgram): Promise<iptv.ISDProgram[] | null> =>
  invokeHubConnection<iptv.ISDProgram[]>('GetSDPrograms', argument);
export const GetSchedules = async (argument: iptv.ISchedule[]): Promise<iptv.ISchedule[] | null> =>
  invokeHubConnection<iptv.ISchedule[]>('GetSchedules', argument);
export const GetSelectedStationIds = async (argument: iptv.IStationIdLineUp[]): Promise<iptv.IStationIdLineUp[] | null> =>
  invokeHubConnection<iptv.IStationIdLineUp[]>('GetSelectedStationIds', argument);
export const GetStationPreviews = async (argument: iptv.IStationPreview[]): Promise<iptv.IStationPreview[] | null> =>
  invokeHubConnection<iptv.IStationPreview[]>('GetStationPreviews', argument);
export const GetStations = async (argument: iptv.IStation[]): Promise<iptv.IStation[] | null> =>
  invokeHubConnection<iptv.IStation[]>('GetStations', argument);
export const GetStatus = async (argument: iptv.ISdStatus): Promise<iptv.ISdStatus | null> =>
  invokeHubConnection<iptv.ISdStatus>('GetStatus', argument);
export const GetEpg = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetEpg');
};
