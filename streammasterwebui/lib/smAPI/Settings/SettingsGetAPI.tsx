import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetIsSystemReady = async (): Promise<boolean | null> => {
  return invokeHubCommand<boolean>('GetIsSystemReady');
};
export const GetSetting = async (argument: iptv.SettingDto): Promise<iptv.SettingDto | null> => invokeHubCommand<iptv.SettingDto>('GetSetting', argument);
export const GetSystemStatus = async (argument: iptv.SdSystemStatus): Promise<iptv.SdSystemStatus | null> =>
  invokeHubCommand<iptv.SdSystemStatus>('GetSystemStatus', argument);
export const LogIn = async (argument: iptv.LogInRequest): Promise<void | null> => {
  await invokeHubCommand<void>('LogIn', argument);
};
