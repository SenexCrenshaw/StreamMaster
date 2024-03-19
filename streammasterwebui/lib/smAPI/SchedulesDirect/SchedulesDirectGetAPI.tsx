/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@lib/common/dataTypes';
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetAvailableCountries = async (argument: iptv.CountryData[]): Promise<iptv.CountryData[] | null> =>
  invokeHubCommand<iptv.CountryData[]>('GetAvailableCountries', argument);
export const GetChannelNames = async (): Promise<void | null> => {
  await invokeHubCommand<void>('GetChannelNames');
};
export const GetHeadends = async (argument: iptv.GetHeadends): Promise<iptv.HeadendDto[] | null> =>
  invokeHubCommand<iptv.HeadendDto[]>('GetHeadends', argument);
export const GetLineupPreviewChannel = async (argument: iptv.GetLineupPreviewChannel): Promise<iptv.LineupPreviewChannel[] | null> =>
  invokeHubCommand<iptv.LineupPreviewChannel[]>('GetLineupPreviewChannel', argument);
export const GetLineups = async (argument: iptv.SubscribedLineup[]): Promise<iptv.SubscribedLineup[] | null> =>
  invokeHubCommand<iptv.SubscribedLineup[]>('GetLineups', argument);
export const GetPagedStationChannelNameSelections = async (argument: iptv.PagedResponseOfStationChannelName): Promise<iptv.StationChannelName[] | null> =>
  invokeHubCommand<iptv.StationChannelName[]>('GetPagedStationChannelNameSelections', argument);
export const GetSelectedStationIds = async (argument: iptv.StationIdLineup[]): Promise<iptv.StationIdLineup[] | null> =>
  invokeHubCommand<iptv.StationIdLineup[]>('GetSelectedStationIds', argument);
export const GetStationChannelMaps = async (argument: iptv.StationChannelMap[]): Promise<iptv.StationChannelMap[] | null> =>
  invokeHubCommand<iptv.StationChannelMap[]>('GetStationChannelMaps', argument);
export const GetStationChannelNameFromDisplayName = async (argument: StringArgument): Promise<iptv.StationChannelName | null> =>
  invokeHubCommand<iptv.StationChannelName>('GetStationChannelNameFromDisplayName', argument);
export const GetStationChannelNamesSimpleQuery = async (argument: iptv.StationChannelName[]): Promise<iptv.StationChannelName[] | null> =>
  invokeHubCommand<iptv.StationChannelName[]>('GetStationChannelNamesSimpleQuery', argument);
export const GetStationPreviews = async (argument: iptv.StationPreview[]): Promise<iptv.StationPreview[] | null> =>
  invokeHubCommand<iptv.StationPreview[]>('GetStationPreviews', argument);
export const GetUserStatus = async (argument: iptv.UserStatus): Promise<iptv.UserStatus | null> => invokeHubCommand<iptv.UserStatus>('GetUserStatus', argument);
