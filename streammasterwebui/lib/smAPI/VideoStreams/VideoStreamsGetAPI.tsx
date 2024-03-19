/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetVideoStream = async (argument: iptv.VideoStreamDto): Promise<iptv.VideoStreamDto | null> =>
  invokeHubCommand<iptv.VideoStreamDto>('GetVideoStream', argument);
export const GetVideoStreamNames = async (argument: iptv.IdName[]): Promise<iptv.IdName[] | null> =>
  invokeHubCommand<iptv.IdName[]>('GetVideoStreamNames', argument);
export const GetPagedVideoStreams = async (argument: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> =>
  invokeHubCommand<iptv.VideoStreamDto[]>('GetPagedVideoStreams', argument);
export const GetVideoStreamStreamGET = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamGET', argument);
};
export const GetVideoStreamStreamGET2 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamGET2', argument);
};
export const GetVideoStreamStreamGET3 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamGET3', argument);
};
export const GetVideoStreamStreamGET4 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamGET4', argument);
};
export const GetVideoStreamInfoFromId = async (argument: iptv.VideoInfo): Promise<iptv.VideoInfo | null> =>
  invokeHubCommand<iptv.VideoInfo>('GetVideoStreamInfoFromId', argument);
export const GetVideoStreamInfoFromUrl = async (argument: iptv.VideoInfo): Promise<iptv.VideoInfo | null> =>
  invokeHubCommand<iptv.VideoInfo>('GetVideoStreamInfoFromUrl', argument);
export const GetVideoStreamNamesAndUrls = async (argument: iptv.IdNameUrl[]): Promise<iptv.IdNameUrl[] | null> =>
  invokeHubCommand<iptv.IdNameUrl[]>('GetVideoStreamNamesAndUrls', argument);
