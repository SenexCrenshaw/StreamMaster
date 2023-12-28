/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@components/selectors/BaseSelector';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetQueueStatus = async (argument: iptv.TaskQueueStatus[]): Promise<iptv.TaskQueueStatus[] | null> =>
  invokeHubConnection<iptv.TaskQueueStatus[]>('GetQueueStatus', argument);
