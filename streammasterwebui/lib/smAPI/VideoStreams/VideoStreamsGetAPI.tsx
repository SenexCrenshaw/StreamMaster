/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
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

export const GetVideoStreamStreamGET = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamStreamGET');
  await hubConnection.invoke('GetVideoStreamStreamGET', arg);
};

export const GetVideoStreamStreamGET2 = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamStreamGET2');
  await hubConnection.invoke('GetVideoStreamStreamGET2', arg);
};

export const GetVideoStreamStreamGET3 = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamStreamGET3');
  await hubConnection.invoke('GetVideoStreamStreamGET3', arg);
};

