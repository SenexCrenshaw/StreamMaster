/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@/lib/iptvApi';
import { invokeHubConnection } from '@/lib/signalr/signalr';

export const GetVideoStreamVideoStreamIds = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetVideoStreamVideoStreamIds', argument);
};
export const GetPagedVideoStreamVideoStreams = async (argument: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> =>
  invokeHubConnection<iptv.VideoStreamDto[]>('GetPagedVideoStreamVideoStreams', argument);
