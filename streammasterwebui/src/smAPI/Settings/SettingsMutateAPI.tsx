import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const UpdateSetting = async (arg: iptv.UpdateSettingRequest): Promise<void> => {
  if (isDebug) console.log('UpdateSetting');
  await hubConnection.invoke('UpdateSetting', arg);
};

