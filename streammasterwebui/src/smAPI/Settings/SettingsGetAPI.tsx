import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const GetIsSystemReady = async (): Promise<void> => {
  await hubConnection.invoke('GetIsSystemReady');
};

export const GetQueueStatus = async (): Promise<iptv.TaskQueueStatusDto[]> => {
  const data = await hubConnection.invoke('GetQueueStatus');
  return data;
};

export const GetSetting = async (): Promise<iptv.SettingDto> => {
  const data = await hubConnection.invoke('GetSetting');
  return data;
};

export const GetSystemStatus = async (): Promise<iptv.SystemStatus> => {
  const data = await hubConnection.invoke('GetSystemStatus');
  return data;
};

export const LogIn = async (arg: iptv.LogInRequest): Promise<void> => {
  await hubConnection.invoke('LogIn', arg);
};

