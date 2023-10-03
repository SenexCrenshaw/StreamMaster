/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@/lib/signalr/signalr';
import type * as iptv from '@/lib/iptvApi';


export const GetChannelGroup = async (arg: iptv.ChannelGroupDto): Promise<iptv.ChannelGroupDto | null> => {
    return await invokeHubConnection<iptv.ChannelGroupDto> ('GetChannelGroup', arg);
};

export const GetChannelGroupIdNames = async (arg: iptv.ChannelGroupIdName[]): Promise<iptv.ChannelGroupIdName[] | null> => {
    return await invokeHubConnection<iptv.ChannelGroupIdName[]> ('GetChannelGroupIdNames', arg);
};

export const GetPagedChannelGroups = async (arg: iptv.PagedResponseOfChannelGroupDto): Promise<iptv.ChannelGroupDto[] | null> => {
    return await invokeHubConnection<iptv.ChannelGroupDto[]> ('GetPagedChannelGroups', arg);
};

export const GetChannelGroupNames = async (): Promise<void | null> => {
    await invokeHubConnection<void> ('GetChannelGroupNames');
};

export const GetChannelGroupsForStreamGroup = async (arg: iptv.GetChannelGroupsForStreamGroupRequest): Promise<iptv.ChannelGroupDto[] | null> => {
    return await invokeHubConnection<iptv.ChannelGroupDto[]> ('GetChannelGroupsForStreamGroup', arg);
};

