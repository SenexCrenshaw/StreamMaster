import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const UpdateSetting = async (arg: iptv.UpdateSettingRequest): Promise<void> => {
  await hubConnection.invoke('UpdateSetting', arg);
};

