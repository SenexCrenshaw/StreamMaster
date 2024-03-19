/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const SyncStreamGroupChannelGroups = async (argument: iptv.SyncStreamGroupChannelGroupsRequest): Promise<iptv.StreamGroupDto | null> =>
  invokeHubCommand<iptv.StreamGroupDto>('SyncStreamGroupChannelGroups', argument);
