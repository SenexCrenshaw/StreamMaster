/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@lib/common/dataTypes';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetFFMPEGProfiles = async (argument: iptv.FFMPEGProfileDtos): Promise<iptv.FFMPEGProfileDtos | null> =>
  invokeHubConnection<iptv.FFMPEGProfileDtos>('GetFFMPEGProfiles', argument);
