/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetStreamGroupVideoStreamIds = async (argument: iptv.VideoStreamIsReadOnly[]): Promise<iptv.VideoStreamIsReadOnly[] | null> =>
  invokeHubCommand<iptv.VideoStreamIsReadOnly[]>('GetStreamGroupVideoStreamIds', argument);
export const GetPagedStreamGroupVideoStreams = async (argument: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> =>
  invokeHubCommand<iptv.VideoStreamDto[]>('GetPagedStreamGroupVideoStreams', argument);
