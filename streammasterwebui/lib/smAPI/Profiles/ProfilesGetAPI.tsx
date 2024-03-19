/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetFFMPEGProfiles = async (argument: iptv.FFMPEGProfileDtos): Promise<iptv.FFMPEGProfileDtos | null> =>
  invokeHubCommand<iptv.FFMPEGProfileDtos>('GetFFMPEGProfiles', argument);
