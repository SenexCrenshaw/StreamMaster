/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetQueueStatus = async (argument: iptv.TaskQueueStatus[]): Promise<iptv.TaskQueueStatus[] | null> =>
  invokeHubCommand<iptv.TaskQueueStatus[]>('GetQueueStatus', argument);
