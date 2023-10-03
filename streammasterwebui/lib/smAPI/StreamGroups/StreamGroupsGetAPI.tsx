/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@/lib/signalr/signalr';
import type * as iptv from '@/lib/iptvApi';


export const GetStreamGroup = async (arg: iptv.StreamGroupDto): Promise<iptv.StreamGroupDto | null> => {
    return await invokeHubConnection<iptv.StreamGroupDto> ('GetStreamGroup', arg);
};

export const GetStreamGroupCapability = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupCapability', arg);
};

export const GetStreamGroupCapability2 = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupCapability2', arg);
};

export const GetStreamGroupCapability3 = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupCapability3', arg);
};

export const GetStreamGroupDiscover = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupDiscover', arg);
};

export const GetStreamGroupEpg = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupEpg', arg);
};

export const GetStreamGroupEpgForGuide = async (arg: iptv.EpgGuide): Promise<iptv.EpgGuide | null> => {
    return await invokeHubConnection<iptv.EpgGuide> ('GetStreamGroupEpgForGuide', arg);
};

export const GetStreamGroupLineUp = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupLineUp', arg);
};

export const GetStreamGroupLineUpStatus = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupLineUpStatus', arg);
};

export const GetStreamGroupM3U = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetStreamGroupM3U', arg);
};

export const GetPagedStreamGroups = async (arg: iptv.PagedResponseOfStreamGroupDto): Promise<iptv.StreamGroupDto[] | null> => {
    return await invokeHubConnection<iptv.StreamGroupDto[]> ('GetPagedStreamGroups', arg);
};

