/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetStreamGroup = async (argument: iptv.StreamGroupDto): Promise<iptv.StreamGroupDto | null> =>
  invokeHubCommand<iptv.StreamGroupDto>('GetStreamGroup', argument);
export const GetVideoStreamStreamFromAutoGET = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamFromAutoGET', argument);
};
export const GetStreamGroupCapability = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupCapability', argument);
};
export const GetStreamGroupCapability2 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupCapability2', argument);
};
export const GetStreamGroupCapability3 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupCapability3', argument);
};
export const GetStreamGroupDiscover = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupDiscover', argument);
};
export const GetStreamGroupEpg = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupEpg', argument);
};
export const GetStreamGroupLineup = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupLineup', argument);
};
export const GetStreamGroupLineupStatus = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupLineupStatus', argument);
};
export const GetStreamGroupM3U = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupM3U', argument);
};
export const GetPagedStreamGroups = async (argument: iptv.PagedResponseOfStreamGroupDto): Promise<iptv.StreamGroupDto[] | null> =>
  invokeHubCommand<iptv.StreamGroupDto[]>('GetPagedStreamGroups', argument);
export const GetStreamGroupVideoStreamUrl = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetStreamGroupVideoStreamUrl', argument);
};
