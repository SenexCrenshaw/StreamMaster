import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";


export const GetVideoStreamVideoStreamIds = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetVideoStreamVideoStreamIds');
  await hubConnection.invoke('GetVideoStreamVideoStreamIds', arg);
};

export const GetPagedVideoStreamVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[]> => {
  if (isDebug) console.log('GetPagedVideoStreamVideoStreams');
  const data = await hubConnection.invoke('GetPagedVideoStreamVideoStreams', arg);
  return data;
};

