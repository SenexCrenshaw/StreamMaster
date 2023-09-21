import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";


export const GetLog = async (arg: iptv.LogsGetLogApiArg): Promise<iptv.LogEntryDto[]> => {
  // if (isDebug) console.log('GetLog');
  const data = await hubConnection.invoke('GetLog', arg);
  return data;
};

