/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const CreateChannelGroup = async (arg: iptv.CreateChannelGroupRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('CreateChannelGroup', arg);
};

export const DeleteAllChannelGroupsFromParameters = async (arg: iptv.DeleteAllChannelGroupsFromParametersRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('DeleteAllChannelGroupsFromParameters', arg);
};

export const DeleteChannelGroup = async (arg: iptv.DeleteChannelGroupRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('DeleteChannelGroup', arg);
};

export const UpdateChannelGroup = async (arg: iptv.UpdateChannelGroupRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('UpdateChannelGroup', arg);
};

export const UpdateChannelGroups = async (arg: iptv.UpdateChannelGroupsRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('UpdateChannelGroups', arg);
};

