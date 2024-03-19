/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetChannelGroup = async (argument: iptv.ChannelGroupDto): Promise<iptv.ChannelGroupDto | null> =>
  invokeHubCommand<iptv.ChannelGroupDto>('GetChannelGroup', argument);
export const GetChannelGroupIdNames = async (argument: iptv.ChannelGroupIdName[]): Promise<iptv.ChannelGroupIdName[] | null> =>
  invokeHubCommand<iptv.ChannelGroupIdName[]>('GetChannelGroupIdNames', argument);
export const GetPagedChannelGroups = async (argument: iptv.PagedResponseOfChannelGroupDto): Promise<iptv.ChannelGroupDto[] | null> =>
  invokeHubCommand<iptv.ChannelGroupDto[]>('GetPagedChannelGroups', argument);
export const GetChannelGroupNames = async (): Promise<void | null> => {
  await invokeHubCommand<void>('GetChannelGroupNames');
};
export const GetChannelGroupsForStreamGroup = async (argument: iptv.GetChannelGroupsForStreamGroupRequest): Promise<iptv.ChannelGroupDto[] | null> =>
  invokeHubCommand<iptv.ChannelGroupDto[]>('GetChannelGroupsForStreamGroup', argument);
