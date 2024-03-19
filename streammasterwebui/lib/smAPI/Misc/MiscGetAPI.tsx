/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetDownloadServiceStatus = async (argument: iptv.ImageDownloadServiceStatus): Promise<iptv.ImageDownloadServiceStatus | null> =>
  invokeHubCommand<iptv.ImageDownloadServiceStatus>('GetDownloadServiceStatus', argument);
export const GetTestM3U = async (argument: number): Promise<void | null> => {
  await invokeHubCommand<void>('GetTestM3U', argument);
};
