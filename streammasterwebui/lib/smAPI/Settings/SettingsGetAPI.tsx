/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@/lib/signalr/signalr';
import type * as iptv from '@/lib/iptvApi';


export const GetIsSystemReady = async (): Promise<void | null> => {
    await invokeHubConnection<void> ('GetIsSystemReady');
};

export const GetQueueStatus = async (arg: iptv.TaskQueueStatusDto[]): Promise<iptv.TaskQueueStatusDto[] | null> => {
    return await invokeHubConnection<iptv.TaskQueueStatusDto[]> ('GetQueueStatus', arg);
};

export const GetSetting = async (arg: iptv.SettingDto): Promise<iptv.SettingDto | null> => {
    return await invokeHubConnection<iptv.SettingDto> ('GetSetting', arg);
};

export const GetSystemStatus = async (arg: iptv.SystemStatus): Promise<iptv.SystemStatus | null> => {
    return await invokeHubConnection<iptv.SystemStatus> ('GetSystemStatus', arg);
};

export const LogIn = async (arg: iptv.LogInRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('LogIn', arg);
};

