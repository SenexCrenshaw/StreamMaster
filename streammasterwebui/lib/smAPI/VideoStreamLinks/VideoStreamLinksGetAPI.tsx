/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetVideoStreamVideoStreamIds = async (arg: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamVideoStreamIds', arg);
};

export const GetPagedVideoStreamVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> => {
  return await invokeHubConnection<iptv.VideoStreamDto[]>('GetPagedVideoStreamVideoStreams', arg);
};
