import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetIsSystemReady = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetIsSystemReady');
};
export const GetSetting = async (argument: iptv.SettingDto): Promise<iptv.SettingDto | null> => invokeHubConnection<iptv.SettingDto>('GetSetting', argument);
export const GetSystemStatus = async (argument: iptv.SdSystemStatus): Promise<iptv.SdSystemStatus | null> =>
  invokeHubConnection<iptv.SdSystemStatus>('GetSystemStatus', argument);
export const LogIn = async (argument: iptv.LogInRequest): Promise<void | null> => {
  await invokeHubConnection<void>('LogIn', argument);
};
