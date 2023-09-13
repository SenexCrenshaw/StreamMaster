import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetCountries = async (arg: iptv.Countries): Promise<iptv.Countries> => {
  const data = await hubConnection.invoke('GetCountries', arg);
  return data;
};

export const GetHeadends = async (arg: iptv.HeadendDto[]): Promise<iptv.HeadendDto[]> => {
  const data = await hubConnection.invoke('GetHeadends', arg);
  return data;
};

export const GetLineup = async (arg: iptv.LineUpResult): Promise<iptv.LineUpResult> => {
  const data = await hubConnection.invoke('GetLineup', arg);
  return data;
};

export const GetLineupPreviews = async (arg: iptv.LineUpPreview[]): Promise<iptv.LineUpPreview[]> => {
  const data = await hubConnection.invoke('GetLineupPreviews', arg);
  return data;
};

export const GetLineups = async (arg: iptv.LineUpsResult): Promise<iptv.LineUpsResult> => {
  const data = await hubConnection.invoke('GetLineups', arg);
  return data;
};

export const GetSchedules = async (arg: iptv.Schedule[]): Promise<iptv.Schedule[]> => {
  const data = await hubConnection.invoke('GetSchedules', arg);
  return data;
};

export const GetStationPreviews = async (arg: iptv.StationPreview[]): Promise<iptv.StationPreview[]> => {
  const data = await hubConnection.invoke('GetStationPreviews', arg);
  return data;
};

export const GetStations = async (arg: iptv.Station[]): Promise<iptv.Station[]> => {
  const data = await hubConnection.invoke('GetStations', arg);
  return data;
};

export const GetStatus = async (arg: iptv.SdStatus): Promise<iptv.SdStatus> => {
  const data = await hubConnection.invoke('GetStatus', arg);
  return data;
};

