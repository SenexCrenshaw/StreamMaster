/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@/lib/iptvApi';
import { invokeHubConnection } from '@/lib/signalr/signalr';

export const SyncStreamGroupChannelGroups = async (argument: iptv.SyncStreamGroupChannelGroupsRequest): Promise<iptv.StreamGroupDto | null> =>
  invokeHubConnection<iptv.StreamGroupDto>('SyncStreamGroupChannelGroups', argument);
