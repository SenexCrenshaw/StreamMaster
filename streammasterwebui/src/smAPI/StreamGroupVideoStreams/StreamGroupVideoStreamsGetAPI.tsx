import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";


export const GetStreamGroupVideoStreamIds = async (arg: iptv.VideoStreamIsReadOnly[]): Promise<iptv.VideoStreamIsReadOnly[]> => {
  const data = await hubConnection.invoke('GetStreamGroupVideoStreamIds', arg);
  return data;
};

export const GetStreamGroupVideoStreams = async (arg: iptv.PagedResponseOfVideoStreamDto): Promise<iptv.VideoStreamDto[]> => {
  const data = await hubConnection.invoke('GetStreamGroupVideoStreams', arg);
  return data;
};

