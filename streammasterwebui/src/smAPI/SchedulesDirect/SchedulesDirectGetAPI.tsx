import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetCountries = async (): Promise<iptv.Countries> => {
  const data = await hubConnection.invoke('GetCountries');
  return data;
};

export const GetHeadends = async (): Promise<iptv.HeadendDto[]> => {
  const data = await hubConnection.invoke('GetHeadends');
  return data;
};

export const GetLineup = async (): Promise<iptv.LineUpResult> => {
  const data = await hubConnection.invoke('GetLineup');
  return data;
};

export const GetLineupPreviews = async (): Promise<iptv.LineUpPreview[]> => {
  const data = await hubConnection.invoke('GetLineupPreviews');
  return data;
};

export const GetLineups = async (): Promise<iptv.LineUpsResult> => {
  const data = await hubConnection.invoke('GetLineups');
  return data;
};

export const GetSchedules = async (): Promise<iptv.Schedule[]> => {
  const data = await hubConnection.invoke('GetSchedules');
  return data;
};

export const GetStationPreviews = async (): Promise<iptv.StationPreview[]> => {
  const data = await hubConnection.invoke('GetStationPreviews');
  return data;
};

export const GetStations = async (): Promise<iptv.Station[]> => {
  const data = await hubConnection.invoke('GetStations');
  return data;
};

export const GetStatus = async (): Promise<iptv.SdStatus> => {
  const data = await hubConnection.invoke('GetStatus');
  return data;
};

