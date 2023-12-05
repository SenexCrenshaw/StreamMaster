/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@components/selectors/BaseSelector';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetPagedStationChannelNameSelections = async (argument: iptv.PagedResponseOfStationChannelName): Promise<iptv.StationChannelName[] | null> =>
  invokeHubConnection<iptv.StationChannelName[]>('GetPagedStationChannelNameSelections', argument);
export const GetStationChannelNameFromDisplayName = async (argument: StringArgument): Promise<iptv.StationChannelName | null> =>
  invokeHubConnection<iptv.StationChannelName>('GetStationChannelNameFromDisplayName', argument);
export const GetStationChannelNamesSimpleQuery = async (argument: iptv.StationChannelName[]): Promise<iptv.StationChannelName[] | null> =>
  invokeHubConnection<iptv.StationChannelName[]>('GetStationChannelNamesSimpleQuery', argument);
export const GetStatus = async (argument: iptv.UserStatus): Promise<iptv.UserStatus | null> => invokeHubConnection<iptv.UserStatus>('GetStatus', argument);
