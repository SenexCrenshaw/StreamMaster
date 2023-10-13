/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@/lib/signalr/signalr';
import type * as iptv from '@/lib/iptvApi';

export const GetAllStatisticsForAllUrls = async (arg: iptv.StreamStatisticsResult[]): Promise<iptv.StreamStatisticsResult[] | null> => {
  return await invokeHubConnection<iptv.StreamStatisticsResult[]>('GetAllStatisticsForAllUrls', arg);
};

export const GetChannelLogoDtos = async (arg: iptv.ChannelLogoDto[]): Promise<iptv.ChannelLogoDto[] | null> => {
  return await invokeHubConnection<iptv.ChannelLogoDto[]>('GetChannelLogoDtos', arg);
};

export const GetVideoStream = async (arg: iptv.VideoStreamDto): Promise<iptv.VideoStreamDto | null> => {
  return await invokeHubConnection<iptv.VideoStreamDto>('GetVideoStream', arg);
};

export const GetPagedVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> => {
  return await invokeHubConnection<iptv.VideoStreamDto[]>('GetPagedVideoStreams', arg);
};

export const GetVideoStreamStreamGET = async (arg: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET', arg);
};

export const GetVideoStreamStreamGET2 = async (arg: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET2', arg);
};

export const GetVideoStreamStreamGET3 = async (arg: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET3', arg);
};
