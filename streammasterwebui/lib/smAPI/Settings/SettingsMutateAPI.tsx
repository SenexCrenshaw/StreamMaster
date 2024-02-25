/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const AddFFMPEGProfile = async (argument: iptv.AddFfmpegProfileRequest): Promise<iptv.UpdateSettingResponse | null> =>
  invokeHubConnection<iptv.UpdateSettingResponse>('AddFFMPEGProfile', argument);
export const RemoveFFMPEGProfile = async (argument: iptv.RemoveFfmpegProfileRequest): Promise<iptv.UpdateSettingResponse | null> =>
  invokeHubConnection<iptv.UpdateSettingResponse>('RemoveFFMPEGProfile', argument);
export const UpdateFFMPEGProfile = async (argument: iptv.UpdateFfmpegProfileRequest): Promise<iptv.UpdateSettingResponse | null> =>
  invokeHubConnection<iptv.UpdateSettingResponse>('UpdateFFMPEGProfile', argument);
export const UpdateSetting = async (argument: iptv.UpdateSettingRequest): Promise<void | null> => {
  await invokeHubConnection<void>('UpdateSetting', argument);
};
