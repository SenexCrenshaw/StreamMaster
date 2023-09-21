import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";


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

