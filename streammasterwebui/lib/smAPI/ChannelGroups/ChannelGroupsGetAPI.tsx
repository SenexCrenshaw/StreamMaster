/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@lib/common/dataTypes';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetChannelGroup = async (argument: iptv.ChannelGroupDto): Promise<iptv.ChannelGroupDto | null> =>
  invokeHubConnection<iptv.ChannelGroupDto>('GetChannelGroup', argument);
export const GetChannelGroupIdNames = async (argument: iptv.ChannelGroupIdName[]): Promise<iptv.ChannelGroupIdName[] | null> =>
  invokeHubConnection<iptv.ChannelGroupIdName[]>('GetChannelGroupIdNames', argument);
export const GetPagedChannelGroups = async (argument: iptv.PagedResponseOfChannelGroupDto): Promise<iptv.ChannelGroupDto[] | null> =>
  invokeHubConnection<iptv.ChannelGroupDto[]>('GetPagedChannelGroups', argument);
export const GetChannelGroupNames = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetChannelGroupNames');
};
export const GetChannelGroupsForStreamGroup = async (argument: iptv.GetChannelGroupsForStreamGroupRequest): Promise<iptv.ChannelGroupDto[] | null> =>
  invokeHubConnection<iptv.ChannelGroupDto[]>('GetChannelGroupsForStreamGroup', argument);
