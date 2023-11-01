/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@/lib/iptvApi';
import { invokeHubConnection } from '@/lib/signalr/signalr';

export const GetStreamGroupVideoStreamIds = async (argument: iptv.VideoStreamIsReadOnly[]): Promise<iptv.VideoStreamIsReadOnly[] | null> =>
  invokeHubConnection<iptv.VideoStreamIsReadOnly[]>('GetStreamGroupVideoStreamIds', argument);
export const GetPagedStreamGroupVideoStreams = async (argument: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> =>
  invokeHubConnection<iptv.VideoStreamDto[]>('GetPagedStreamGroupVideoStreams', argument);
