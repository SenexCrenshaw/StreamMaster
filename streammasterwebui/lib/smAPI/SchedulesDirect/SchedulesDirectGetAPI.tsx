/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetCountries = async (arg: iptv.Countries): Promise<iptv.Countries | null> => {
  return await invokeHubConnection<iptv.Countries>('GetCountries', arg);
};

export const GetHeadends = async (arg: iptv.HeadendDto[]): Promise<iptv.HeadendDto[] | null> => {
  return await invokeHubConnection<iptv.HeadendDto[]>('GetHeadends', arg);
};

export const GetLineup = async (arg: iptv.LineUpResult): Promise<iptv.LineUpResult | null> => {
  return await invokeHubConnection<iptv.LineUpResult>('GetLineup', arg);
};

export const GetLineupPreviews = async (arg: iptv.LineUpPreview[]): Promise<iptv.LineUpPreview[] | null> => {
  return await invokeHubConnection<iptv.LineUpPreview[]>('GetLineupPreviews', arg);
};

export const GetLineups = async (arg: iptv.LineUpsResult): Promise<iptv.LineUpsResult | null> => {
  return await invokeHubConnection<iptv.LineUpsResult>('GetLineups', arg);
};

export const GetSchedules = async (arg: iptv.Schedule[]): Promise<iptv.Schedule[] | null> => {
  return await invokeHubConnection<iptv.Schedule[]>('GetSchedules', arg);
};

export const GetSelectedStationIds = async (arg: iptv.StationIdLineUp[]): Promise<iptv.StationIdLineUp[] | null> => {
  return await invokeHubConnection<iptv.StationIdLineUp[]>('GetSelectedStationIds', arg);
};

export const GetStationPreviews = async (arg: iptv.StationPreview[]): Promise<iptv.StationPreview[] | null> => {
  return await invokeHubConnection<iptv.StationPreview[]>('GetStationPreviews', arg);
};

export const GetStations = async (arg: iptv.Station[]): Promise<iptv.Station[] | null> => {
  return await invokeHubConnection<iptv.Station[]>('GetStations', arg);
};

export const GetStatus = async (arg: iptv.SdStatus): Promise<iptv.SdStatus | null> => {
  return await invokeHubConnection<iptv.SdStatus>('GetStatus', arg);
};

export const GetEpg = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetEpg');
};
