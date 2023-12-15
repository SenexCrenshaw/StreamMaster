/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@components/selectors/BaseSelector';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetDownloadServiceStatus = async (argument: iptv.ImageDownloadServiceStatus): Promise<iptv.ImageDownloadServiceStatus | null> =>
  invokeHubConnection<iptv.ImageDownloadServiceStatus>('GetDownloadServiceStatus', argument);
