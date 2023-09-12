import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const AutoMatchIconToStreams = async (arg: iptv.AutoMatchIconToStreamsRequest): Promise<void> => {
  await hubConnection.invoke('AutoMatchIconToStreams', arg);
};

