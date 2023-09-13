import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetIsSystemReady = async (): Promise<void> => {
  await hubConnection.invoke('GetIsSystemReady');
};

export const GetQueueStatus = async (arg: iptv.TaskQueueStatusDto[]): Promise<iptv.TaskQueueStatusDto[]> => {
  const data = await hubConnection.invoke('GetQueueStatus', arg);
  return data;
};

export const GetSetting = async (arg: iptv.SettingDto): Promise<iptv.SettingDto> => {
  const data = await hubConnection.invoke('GetSetting', arg);
  return data;
};

export const GetSystemStatus = async (arg: iptv.SystemStatus): Promise<iptv.SystemStatus> => {
  const data = await hubConnection.invoke('GetSystemStatus', arg);
  return data;
};

export const LogIn = async (arg: iptv.LogInRequest): Promise<void> => {
  await hubConnection.invoke('LogIn', arg);
};

