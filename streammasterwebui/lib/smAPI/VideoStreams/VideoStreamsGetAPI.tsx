import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetAllStatisticsForAllUrls = async (arg: iptv.StreamStatisticsResult[]): Promise<iptv.StreamStatisticsResult[]> => {
  if (isDebug) console.log('GetAllStatisticsForAllUrls');
  const data = await hubConnection.invoke('GetAllStatisticsForAllUrls', arg);
  return data;
};

export const GetChannelLogoDtos = async (arg: iptv.ChannelLogoDto[]): Promise<iptv.ChannelLogoDto[]> => {
  if (isDebug) console.log('GetChannelLogoDtos');
  const data = await hubConnection.invoke('GetChannelLogoDtos', arg);
  return data;
};

export const GetVideoStream = async (arg: iptv.VideoStreamDto): Promise<iptv.VideoStreamDto> => {
  if (isDebug) console.log('GetVideoStream');
  const data = await hubConnection.invoke('GetVideoStream', arg);
  return data;
};

export const GetPagedVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[]> => {
  if (isDebug) console.log('GetPagedVideoStreams');
  const data = await hubConnection.invoke('GetPagedVideoStreams', arg);
  return data;
};

export const GetVideoStreamStream = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamStream');
  await hubConnection.invoke('GetVideoStreamStream', arg);
};

export const GetVideoStreamStream2 = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamStream2');
  await hubConnection.invoke('GetVideoStreamStream2', arg);
};

export const GetVideoStreamStream3 = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamStream3');
  await hubConnection.invoke('GetVideoStreamStream3', arg);
};

