import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetStreamGroupVideoStreamIds = async (arg: iptv.VideoStreamIsReadOnly[]): Promise<iptv.VideoStreamIsReadOnly[]> => {
  if (isDebug) console.log('GetStreamGroupVideoStreamIds');
  const data = await hubConnection.invoke('GetStreamGroupVideoStreamIds', arg);
  return data;
};

export const GetPagedStreamGroupVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[]> => {
  if (isDebug) console.log('GetPagedStreamGroupVideoStreams');
  const data = await hubConnection.invoke('GetPagedStreamGroupVideoStreams', arg);
  return data;
};

