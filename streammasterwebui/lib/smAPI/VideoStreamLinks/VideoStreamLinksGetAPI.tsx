/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetVideoStreamVideoStreamIds = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetVideoStreamVideoStreamIds', arg);
};

export const GetPagedVideoStreamVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[] | null> => {
    return await invokeHubConnection<iptv.VideoStreamDto[]> ('GetPagedVideoStreamVideoStreams', arg);
};

