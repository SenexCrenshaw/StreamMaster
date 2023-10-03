/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const SyncStreamGroupChannelGroups = async (arg: iptv.SyncStreamGroupChannelGroupsRequest): Promise<iptv.StreamGroupDto | null> => {
    return await invokeHubConnection<iptv.StreamGroupDto> ('SyncStreamGroupChannelGroups', arg);
};

