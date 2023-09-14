import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";


export const GetAllStatisticsForAllUrls = async (arg: iptv.StreamStatisticsResult[]): Promise<iptv.StreamStatisticsResult[]> => {
  const data = await hubConnection.invoke('GetAllStatisticsForAllUrls', arg);
  return data;
};

export const GetChannelLogoDtos = async (arg: iptv.ChannelLogoDto[]): Promise<iptv.ChannelLogoDto[]> => {
  const data = await hubConnection.invoke('GetChannelLogoDtos', arg);
  return data;
};

export const GetVideoStream = async (arg: iptv.VideoStreamDto): Promise<iptv.VideoStreamDto> => {
  const data = await hubConnection.invoke('GetVideoStream', arg);
  return data;
};

export const GetVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[]> => {
  const data = await hubConnection.invoke('GetVideoStreams', arg);
  return data;
};

export const GetVideoStreamStream = async (arg: string): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream', arg);
};

export const GetVideoStreamStream2 = async (arg: string): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream2', arg);
};

export const GetVideoStreamStream3 = async (arg: string): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamStream3', arg);
};

