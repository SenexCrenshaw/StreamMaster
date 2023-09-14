import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";


export const GetVideoStreamVideoStreamIds = async (arg: string): Promise<void> => {
  await hubConnection.invoke('GetVideoStreamVideoStreamIds', arg);
};

export const GetVideoStreamVideoStreams = async (arg: iptv.PagedResponseOfChildVideoStreamDto): Promise<iptv.ChildVideoStreamDto[]> => {
  const data = await hubConnection.invoke('GetVideoStreamVideoStreams', arg);
  return data;
};

