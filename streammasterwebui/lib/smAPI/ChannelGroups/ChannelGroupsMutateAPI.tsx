/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const CreateChannelGroup = async (argument: iptv.CreateChannelGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('CreateChannelGroup', argument);
};
export const DeleteAllChannelGroupsFromParameters = async (argument: iptv.DeleteAllChannelGroupsFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('DeleteAllChannelGroupsFromParameters', argument);
};
export const DeleteChannelGroup = async (argument: iptv.DeleteChannelGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('DeleteChannelGroup', argument);
};
export const UpdateChannelGroup = async (argument: iptv.UpdateChannelGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('UpdateChannelGroup', argument);
};
export const UpdateChannelGroups = async (argument: iptv.UpdateChannelGroupsRequest): Promise<void | null> => {
  await invokeHubCommand<void>('UpdateChannelGroups', argument);
};
