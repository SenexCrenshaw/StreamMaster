/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetChannelGroupsFromStreamGroup = async (argument: iptv.ChannelGroupDto[]): Promise<iptv.ChannelGroupDto[] | null> =>
  invokeHubCommand<iptv.ChannelGroupDto[]>('GetChannelGroupsFromStreamGroup', argument);
