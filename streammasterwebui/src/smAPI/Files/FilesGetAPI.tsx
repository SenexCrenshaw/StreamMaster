import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetFile = async (arg: iptv.SmFileTypes): Promise<void> => {
  await hubConnection.invoke('GetFile', arg);
};

