/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@components/selectors/BaseSelector';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetChannelGroupsFromStreamGroup = async (argument: iptv.ChannelGroupDto[]): Promise<iptv.ChannelGroupDto[] | null> =>
  invokeHubConnection<iptv.ChannelGroupDto[]>('GetChannelGroupsFromStreamGroup', argument);
