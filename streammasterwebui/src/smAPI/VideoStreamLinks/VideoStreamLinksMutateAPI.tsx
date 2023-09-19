import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const AddVideoStreamToVideoStream = async (arg: iptv.AddVideoStreamToVideoStreamRequest): Promise<void> => {
  if (isDebug) console.log('AddVideoStreamToVideoStream');
  await hubConnection.invoke('AddVideoStreamToVideoStream', arg);
};

export const RemoveVideoStreamFromVideoStream = async (arg: iptv.RemoveVideoStreamFromVideoStreamRequest): Promise<void> => {
  if (isDebug) console.log('RemoveVideoStreamFromVideoStream');
  await hubConnection.invoke('RemoveVideoStreamFromVideoStream', arg);
};

