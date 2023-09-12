import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const AddVideoStreamToVideoStream = async (arg: iptv.AddVideoStreamToVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('AddVideoStreamToVideoStream', arg);
};

export const RemoveVideoStreamFromVideoStream = async (arg: iptv.RemoveVideoStreamFromVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('RemoveVideoStreamFromVideoStream', arg);
};

