import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetAllStatisticsForAllUrls = async (): Promise<iptv.StreamStatisticsResult[]> => {
  const data = await hubConnection.invoke('GetAllStatisticsForAllUrls');
  return data;
};

export const GetChannelLogoDtos = async (): Promise<iptv.ChannelLogoDto[]> => {
  const data = await hubConnection.invoke('GetChannelLogoDtos');
  return data;
};

export const GetVideoStream = async (): Promise<iptv.VideoStreamDto> => {
  const data = await hubConnection.invoke('GetVideoStream');
  return data;
};

export const GetVideoStreams = async (): Promise<iptv.PagedResponseOfVideoStreamDto> => {
  const data = await hubConnection.invoke('GetVideoStreams');
  return data;
};

export const GetVideoStreamStream = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream');
};

export const GetVideoStreamStream2 = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream2');
};

export const GetVideoStreamStream3 = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream3');
};

