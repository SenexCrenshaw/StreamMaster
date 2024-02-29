/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const AddFFMPEGProfile = async (argument: iptv.AddFFMPEGProfileRequest): Promise<iptv.UpdateSettingResponse | null> =>
  invokeHubConnection<iptv.UpdateSettingResponse>('AddFFMPEGProfile', argument);
export const RemoveFFMPEGProfile = async (argument: iptv.RemoveFFMPEGProfileRequest): Promise<iptv.UpdateSettingResponse | null> =>
  invokeHubConnection<iptv.UpdateSettingResponse>('RemoveFFMPEGProfile', argument);
export const UpdateFFMPEGProfile = async (argument: iptv.UpdateFFMPEGProfileRequest): Promise<iptv.UpdateSettingResponse | null> =>
  invokeHubConnection<iptv.UpdateSettingResponse>('UpdateFFMPEGProfile', argument);
