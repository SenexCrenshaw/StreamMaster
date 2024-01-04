/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetAllStatisticsForAllUrls = async (argument: iptv.StreamStatisticsResult[]): Promise<iptv.StreamStatisticsResult[] | null> =>
  invokeHubConnection<iptv.StreamStatisticsResult[]>('GetAllStatisticsForAllUrls', argument);
export const GetVideoStream = async (argument: iptv.VideoStreamDto): Promise<iptv.VideoStreamDto | null> =>
  invokeHubConnection<iptv.VideoStreamDto>('GetVideoStream', argument);
export const GetVideoStreamNames = async (argument: iptv.IdName[]): Promise<iptv.IdName[] | null> =>
  invokeHubConnection<iptv.IdName[]>('GetVideoStreamNames', argument);
export const GetPagedVideoStreams = async (argument: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> =>
  invokeHubConnection<iptv.VideoStreamDto[]>('GetPagedVideoStreams', argument);
export const GetVideoStreamStreamGET = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET', argument);
};
export const GetVideoStreamStreamGET2 = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET2', argument);
};
export const GetVideoStreamStreamGET3 = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamStreamGET3', argument);
};
export const GetVideoStreamInfoFromId = async (argument: string): Promise<iptv.VideoInfo | null> =>
  invokeHubConnection<iptv.VideoInfo>('GetVideoStreamInfoFromId', argument);
export const GetVideoStreamInfoFromUrl = async (argument: iptv.VideoInfo): Promise<iptv.VideoInfo | null> =>
  invokeHubConnection<iptv.VideoInfo>('GetVideoStreamInfoFromUrl', argument);
