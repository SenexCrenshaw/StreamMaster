/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@lib/common/dataTypes';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetClientStatistics = async (argument: iptv.ClientStreamingStatistics[]): Promise<iptv.ClientStreamingStatistics[] | null> =>
  invokeHubConnection<iptv.ClientStreamingStatistics[]>('GetClientStatistics', argument);
export const GetInputStatistics = async (argument: iptv.InputStreamingStatistics[]): Promise<iptv.InputStreamingStatistics[] | null> =>
  invokeHubConnection<iptv.InputStreamingStatistics[]>('GetInputStatistics', argument);
