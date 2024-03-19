/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetVideoStreamVideoStreamIds = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamVideoStreamIds', argument);
};
export const GetPagedVideoStreamVideoStreams = async (argument: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> =>
  invokeHubCommand<iptv.VideoStreamDto[]>('GetPagedVideoStreamVideoStreams', argument);
