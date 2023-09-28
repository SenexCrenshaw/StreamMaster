import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetIsSystemReady = async (): Promise<void> => {
  if (isDebug) console.log('GetIsSystemReady');
  await hubConnection.invoke('GetIsSystemReady');
};

export const GetQueueStatus = async (arg: iptv.TaskQueueStatusDto[]): Promise<iptv.TaskQueueStatusDto[]> => {
  if (isDebug) console.log('GetQueueStatus');
  const data = await hubConnection.invoke('GetQueueStatus', arg);
  return data;
};

export const GetSetting = async (arg: iptv.SettingDto): Promise<iptv.SettingDto> => {
  if (isDebug) console.log('GetSetting');
  const data = await hubConnection.invoke('GetSetting', arg);
  return data;
};

export const GetSystemStatus = async (arg: iptv.SystemStatus): Promise<iptv.SystemStatus> => {
  if (isDebug) console.log('GetSystemStatus');
  const data = await hubConnection.invoke('GetSystemStatus', arg);
  return data;
};

export const LogIn = async (arg: iptv.LogInRequest): Promise<void> => {
  if (isDebug) console.log('LogIn');
  await hubConnection.invoke('LogIn', arg);
};

