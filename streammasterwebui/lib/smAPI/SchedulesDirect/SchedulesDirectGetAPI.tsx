/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetCountries = async (arg: iptv.Countries): Promise<iptv.Countries> => {
  if (isDebug) console.log('GetCountries');
  const data = await hubConnection.invoke('GetCountries', arg);
  return data;
};

export const GetHeadends = async (arg: iptv.HeadendDto[]): Promise<iptv.HeadendDto[]> => {
  if (isDebug) console.log('GetHeadends');
  const data = await hubConnection.invoke('GetHeadends', arg);
  return data;
};

export const GetLineup = async (arg: iptv.LineUpResult): Promise<iptv.LineUpResult> => {
  if (isDebug) console.log('GetLineup');
  const data = await hubConnection.invoke('GetLineup', arg);
  return data;
};

export const GetLineupPreviews = async (arg: iptv.LineUpPreview[]): Promise<iptv.LineUpPreview[]> => {
  if (isDebug) console.log('GetLineupPreviews');
  const data = await hubConnection.invoke('GetLineupPreviews', arg);
  return data;
};

export const GetLineups = async (arg: iptv.LineUpsResult): Promise<iptv.LineUpsResult> => {
  if (isDebug) console.log('GetLineups');
  const data = await hubConnection.invoke('GetLineups', arg);
  return data;
};

export const GetSchedules = async (arg: iptv.Schedule[]): Promise<iptv.Schedule[]> => {
  if (isDebug) console.log('GetSchedules');
  const data = await hubConnection.invoke('GetSchedules', arg);
  return data;
};

export const GetStationPreviews = async (arg: iptv.StationPreview[]): Promise<iptv.StationPreview[]> => {
  if (isDebug) console.log('GetStationPreviews');
  const data = await hubConnection.invoke('GetStationPreviews', arg);
  return data;
};

export const GetStations = async (arg: iptv.Station[]): Promise<iptv.Station[]> => {
  if (isDebug) console.log('GetStations');
  const data = await hubConnection.invoke('GetStations', arg);
  return data;
};

export const GetStatus = async (arg: iptv.SdStatus): Promise<iptv.SdStatus> => {
  if (isDebug) console.log('GetStatus');
  const data = await hubConnection.invoke('GetStatus', arg);
  return data;
};

