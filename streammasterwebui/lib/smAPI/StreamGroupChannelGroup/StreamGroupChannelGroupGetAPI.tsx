/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';


export const GetChannelGroupsFromStreamGroup = async (arg: iptv.ChannelGroupDto[]): Promise<iptv.ChannelGroupDto[] | null> => {
    return await invokeHubConnection<iptv.ChannelGroupDto[]> ('GetChannelGroupsFromStreamGroup', arg);
};

