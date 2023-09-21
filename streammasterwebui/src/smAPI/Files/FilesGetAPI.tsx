import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";


export const GetFile = async (arg: string): Promise<void> => {
  if (isDebug) console.log('GetFile');
  await hubConnection.invoke('GetFile', arg);
};

