/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetStreamGroup = async (argument: iptv.StreamGroupDto): Promise<iptv.StreamGroupDto | null> =>
  invokeHubConnection<iptv.StreamGroupDto>('GetStreamGroup', argument);
export const GetStreamGroupCapability = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupCapability', argument);
};
export const GetStreamGroupCapability2 = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupCapability2', argument);
};
export const GetStreamGroupCapability3 = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupCapability3', argument);
};
export const GetStreamGroupDiscover = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupDiscover', argument);
};
export const GetStreamGroupEpg = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupEpg', argument);
};
export const GetStreamGroupLineup = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupLineup', argument);
};
export const GetStreamGroupLineupStatus = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupLineupStatus', argument);
};
export const GetStreamGroupM3U = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupM3U', argument);
};
export const GetPagedStreamGroups = async (argument: iptv.PagedResponseOfStreamGroupDto): Promise<iptv.StreamGroupDto[] | null> =>
  invokeHubConnection<iptv.StreamGroupDto[]>('GetPagedStreamGroups', argument);
export const GetStreamGroupVideoStreamUrl = async (argument: string): Promise<void | null> => {
  await invokeHubConnection<void>('GetStreamGroupVideoStreamUrl', argument);
};
