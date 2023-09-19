import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const AutoMatchIconToStreams = async (arg: iptv.AutoMatchIconToStreamsRequest): Promise<void> => {
  if (isDebug) console.log('AutoMatchIconToStreams');
  await hubConnection.invoke('AutoMatchIconToStreams', arg);
};

