import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetStreamGroupVideoStreamIds = async (): Promise<iptv.VideoStreamIsReadOnly[]> => {
  const data = await hubConnection.invoke('GetStreamGroupVideoStreamIds');
  return data;
};

export const GetStreamGroupVideoStreams = async (): Promise<iptv.PagedResponseOfVideoStreamDto> => {
  const data = await hubConnection.invoke('GetStreamGroupVideoStreams');
  return data;
};

