/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@components/selectors/BaseSelector';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const AddStation = async (argument: iptv.AddStation): Promise<void | null> => {
  await invokeHubConnection<void>('AddStation', argument);
};
export const GetAvailableCountries = async (argument: iptv.CountryData[]): Promise<iptv.CountryData[] | null> =>
  invokeHubConnection<iptv.CountryData[]>('GetAvailableCountries', argument);
export const GetChannelNames = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetChannelNames');
};
export const GetHeadends = async (argument: iptv.HeadendDto[]): Promise<iptv.HeadendDto[] | null> =>
  invokeHubConnection<iptv.HeadendDto[]>('GetHeadends', argument);
export const GetLineups = async (argument: iptv.SubscribedLineup[]): Promise<iptv.SubscribedLineup[] | null> =>
  invokeHubConnection<iptv.SubscribedLineup[]>('GetLineups', argument);
export const GetPagedStationChannelNameSelections = async (argument: iptv.PagedResponseOfStationChannelName): Promise<iptv.StationChannelName[] | null> =>
  invokeHubConnection<iptv.StationChannelName[]>('GetPagedStationChannelNameSelections', argument);
export const GetSelectedStationIds = async (argument: iptv.StationIdLineup[]): Promise<iptv.StationIdLineup[] | null> =>
  invokeHubConnection<iptv.StationIdLineup[]>('GetSelectedStationIds', argument);
export const GetStationChannelMaps = async (argument: iptv.StationChannelMap[]): Promise<iptv.StationChannelMap[] | null> =>
  invokeHubConnection<iptv.StationChannelMap[]>('GetStationChannelMaps', argument);
export const GetStationChannelNameFromDisplayName = async (argument: StringArgument): Promise<iptv.StationChannelName | null> =>
  invokeHubConnection<iptv.StationChannelName>('GetStationChannelNameFromDisplayName', argument);
export const GetStationChannelNamesSimpleQuery = async (argument: iptv.StationChannelName[]): Promise<iptv.StationChannelName[] | null> =>
  invokeHubConnection<iptv.StationChannelName[]>('GetStationChannelNamesSimpleQuery', argument);
export const GetStationPreviews = async (argument: iptv.StationPreview[]): Promise<iptv.StationPreview[] | null> =>
  invokeHubConnection<iptv.StationPreview[]>('GetStationPreviews', argument);
export const GetStatus = async (argument: iptv.UserStatus): Promise<iptv.UserStatus | null> =>
  invokeHubConnection<iptv.UserStatus>('GetStatus', argument);
export const RemoveStation = async (argument: iptv.RemoveStation): Promise<void | null> => {
  await invokeHubConnection<void>('RemoveStation', argument);
};
