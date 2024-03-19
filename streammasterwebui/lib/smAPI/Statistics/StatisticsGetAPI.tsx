/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetClientStatistics = async (argument: iptv.ClientStreamingStatistics[]): Promise<iptv.ClientStreamingStatistics[] | null> =>
  invokeHubCommand<iptv.ClientStreamingStatistics[]>('GetClientStatistics', argument);
export const GetInputStatistics = async (argument: iptv.InputStreamingStatistics[]): Promise<iptv.InputStreamingStatistics[] | null> =>
  invokeHubCommand<iptv.InputStreamingStatistics[]>('GetInputStatistics', argument);
