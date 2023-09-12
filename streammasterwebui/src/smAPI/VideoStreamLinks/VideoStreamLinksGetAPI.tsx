import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetVideoStreamVideoStreamIds = async (): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamVideoStreamIds');
};

export const GetVideoStreamVideoStreams = async (): Promise<iptv.PagedResponseOfChildVideoStreamDto> => {
  const data = await hubConnection.invoke('GetVideoStreamVideoStreams');
  return data;
};

