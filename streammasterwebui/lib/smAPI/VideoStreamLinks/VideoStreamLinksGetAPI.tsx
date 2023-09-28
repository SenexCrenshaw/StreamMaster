import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetVideoStreamVideoStreamIds = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamVideoStreamIds');
  await hubConnection.invoke('GetVideoStreamVideoStreamIds', arg);
};

export const GetPagedVideoStreamVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[]> => {
  if (isDebug) console.log('GetPagedVideoStreamVideoStreams');
  const data = await hubConnection.invoke('GetPagedVideoStreamVideoStreams', arg);
  return data;
};

