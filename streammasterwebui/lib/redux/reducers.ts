import { combineReducers } from 'redux';
import currentSettingRequest from '@lib/redux/hooks/currentSettingRequest';
import filters from '@lib/redux/hooks/filters';
import GetAvailableCountriesReducer from '@lib/smAPI/SchedulesDirect/GetAvailableCountriesSlice';
import GetChannelGroupsReducer from '@lib/smAPI/ChannelGroups/GetChannelGroupsSlice';
import GetChannelGroupsFromSMChannelsReducer from '@lib/smAPI/ChannelGroups/GetChannelGroupsFromSMChannelsSlice';
import GetChannelStreamingStatisticsReducer from '@lib/smAPI/Statistics/GetChannelStreamingStatisticsSlice';
import GetClientStreamingStatisticsReducer from '@lib/smAPI/Statistics/GetClientStreamingStatisticsSlice';
import GetEPGColorsReducer from '@lib/smAPI/EPG/GetEPGColorsSlice';
import GetEPGFilePreviewByIdReducer from '@lib/smAPI/EPGFiles/GetEPGFilePreviewByIdSlice';
import GetEPGFilesReducer from '@lib/smAPI/EPGFiles/GetEPGFilesSlice';
import GetEPGNextEPGNumberReducer from '@lib/smAPI/EPGFiles/GetEPGNextEPGNumberSlice';
import GetHeadendsReducer from '@lib/smAPI/SchedulesDirect/GetHeadendsSlice';
import GetIconsReducer from '@lib/smAPI/Icons/GetIconsSlice';
import GetIsSystemReadyReducer from '@lib/smAPI/General/GetIsSystemReadySlice';
import GetLineupPreviewChannelReducer from '@lib/smAPI/SchedulesDirect/GetLineupPreviewChannelSlice';
import GetLineupsReducer from '@lib/smAPI/SchedulesDirect/GetLineupsSlice';
import GetM3UFileNamesReducer from '@lib/smAPI/M3UFiles/GetM3UFileNamesSlice';
import GetM3UFilesReducer from '@lib/smAPI/M3UFiles/GetM3UFilesSlice';
import GetOutputProfileReducer from '@lib/smAPI/Profiles/GetOutputProfileSlice';
import GetOutputProfilesReducer from '@lib/smAPI/Profiles/GetOutputProfilesSlice';
import GetPagedChannelGroupsReducer from '@lib/smAPI/ChannelGroups/GetPagedChannelGroupsSlice';
import GetPagedEPGFilesReducer from '@lib/smAPI/EPGFiles/GetPagedEPGFilesSlice';
import GetPagedM3UFilesReducer from '@lib/smAPI/M3UFiles/GetPagedM3UFilesSlice';
import GetPagedSMChannelsReducer from '@lib/smAPI/SMChannels/GetPagedSMChannelsSlice';
import GetPagedSMStreamsReducer from '@lib/smAPI/SMStreams/GetPagedSMStreamsSlice';
import GetPagedStreamGroupsReducer from '@lib/smAPI/StreamGroups/GetPagedStreamGroupsSlice';
import GetSelectedStationIdsReducer from '@lib/smAPI/SchedulesDirect/GetSelectedStationIdsSlice';
import GetSettingsReducer from '@lib/smAPI/Settings/GetSettingsSlice';
import GetSMChannelReducer from '@lib/smAPI/SMChannels/GetSMChannelSlice';
import GetSMChannelNamesReducer from '@lib/smAPI/SMChannels/GetSMChannelNamesSlice';
import GetSMChannelStreamsReducer from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsSlice';
import GetSMTasksReducer from '@lib/smAPI/SMTasks/GetSMTasksSlice';
import GetStationChannelNamesReducer from '@lib/smAPI/SchedulesDirect/GetStationChannelNamesSlice';
import GetStationPreviewsReducer from '@lib/smAPI/SchedulesDirect/GetStationPreviewsSlice';
import GetStreamGroupReducer from '@lib/smAPI/StreamGroups/GetStreamGroupSlice';
import GetStreamGroupProfilesReducer from '@lib/smAPI/StreamGroups/GetStreamGroupProfilesSlice';
import GetStreamGroupsReducer from '@lib/smAPI/StreamGroups/GetStreamGroupsSlice';
import GetStreamGroupSMChannelsReducer from '@lib/smAPI/StreamGroupSMChannelLinks/GetStreamGroupSMChannelsSlice';
import GetStreamingStatisticsForChannelReducer from '@lib/smAPI/Statistics/GetStreamingStatisticsForChannelSlice';
import GetStreamStreamingStatisticsReducer from '@lib/smAPI/Statistics/GetStreamStreamingStatisticsSlice';
import GetSystemStatusReducer from '@lib/smAPI/General/GetSystemStatusSlice';
import GetTaskIsRunningReducer from '@lib/smAPI/General/GetTaskIsRunningSlice';
import GetVideoInfoFromIdReducer from '@lib/smAPI/SMChannels/GetVideoInfoFromIdSlice';
import GetVideoProfilesReducer from '@lib/smAPI/Profiles/GetVideoProfilesSlice';
import isTrue from '@lib/redux/hooks/isTrue';
import loading from '@lib/redux/hooks/loading';
import messages from '@lib/redux/hooks/messages';
import queryAdditionalFilters from '@lib/redux/hooks/queryAdditionalFilters';
import queryFilter from '@lib/redux/hooks/queryFilter';
import selectAll from '@lib/redux/hooks/selectAll';
import selectedCountry from '@lib/redux/hooks/selectedCountry';
import selectedItems from '@lib/redux/hooks/selectedItems';
import selectedPostalCode from '@lib/redux/hooks/selectedPostalCode';
import selectedSMChannel from '@lib/redux/hooks/selectedSMChannel';
import selectedSMStream from '@lib/redux/hooks/selectedSMStream';
import selectedSMStreams from '@lib/redux/hooks/selectedSMStreams';
import selectedStreamGroup from '@lib/redux/hooks/selectedStreamGroup';
import showHidden from '@lib/redux/hooks/showHidden';
import showSelections from '@lib/redux/hooks/showSelections';
import sortInfo from '@lib/redux/hooks/sortInfo';
import stringValue from '@lib/redux/hooks/stringValue';
import updateSettingRequest from '@lib/redux/hooks/updateSettingRequest';
import { persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';

const filtersConfig = {
  key: 'filters',
  storage
};
const isTrueConfig = {
  key: 'isTrue',
  storage
};
const queryAdditionalFiltersConfig = {
  key: 'queryAdditionalFilters',
  storage
};
const queryFilterConfig = {
  key: 'queryFilter',
  storage
};
const selectAllConfig = {
  key: 'selectAll',
  storage
};
const selectedCountryConfig = {
  key: 'selectedCountry',
  storage
};
const selectedItemsConfig = {
  key: 'selectedItems',
  storage
};
const selectedPostalCodeConfig = {
  key: 'selectedPostalCode',
  storage
};
const selectedSMChannelConfig = {
  key: 'selectedSMChannel',
  storage
};
const selectedSMStreamConfig = {
  key: 'selectedSMStream',
  storage
};
const selectedSMStreamsConfig = {
  key: 'selectedSMStreams',
  storage
};
const selectedStreamGroupConfig = {
  key: 'selectedStreamGroup',
  storage
};
const showHiddenConfig = {
  key: 'showHidden',
  storage
};
const showSelectionsConfig = {
  key: 'showSelections',
  storage
};
const sortInfoConfig = {
  key: 'sortInfo',
  storage
};
const stringValueConfig = {
  key: 'stringValue',
  storage
};
export const rootReducer = combineReducers({
  currentSettingRequest: currentSettingRequest,
  filters: persistReducer(filtersConfig, filters),
  GetAvailableCountries: GetAvailableCountriesReducer,
  GetChannelGroups: GetChannelGroupsReducer,
  GetChannelGroupsFromSMChannels: GetChannelGroupsFromSMChannelsReducer,
  GetChannelStreamingStatistics: GetChannelStreamingStatisticsReducer,
  GetClientStreamingStatistics: GetClientStreamingStatisticsReducer,
  GetEPGColors: GetEPGColorsReducer,
  GetEPGFilePreviewById: GetEPGFilePreviewByIdReducer,
  GetEPGFiles: GetEPGFilesReducer,
  GetEPGNextEPGNumber: GetEPGNextEPGNumberReducer,
  GetHeadends: GetHeadendsReducer,
  GetIcons: GetIconsReducer,
  GetIsSystemReady: GetIsSystemReadyReducer,
  GetLineupPreviewChannel: GetLineupPreviewChannelReducer,
  GetLineups: GetLineupsReducer,
  GetM3UFileNames: GetM3UFileNamesReducer,
  GetM3UFiles: GetM3UFilesReducer,
  GetOutputProfile: GetOutputProfileReducer,
  GetOutputProfiles: GetOutputProfilesReducer,
  GetPagedChannelGroups: GetPagedChannelGroupsReducer,
  GetPagedEPGFiles: GetPagedEPGFilesReducer,
  GetPagedM3UFiles: GetPagedM3UFilesReducer,
  GetPagedSMChannels: GetPagedSMChannelsReducer,
  GetPagedSMStreams: GetPagedSMStreamsReducer,
  GetPagedStreamGroups: GetPagedStreamGroupsReducer,
  GetSelectedStationIds: GetSelectedStationIdsReducer,
  GetSettings: GetSettingsReducer,
  GetSMChannel: GetSMChannelReducer,
  GetSMChannelNames: GetSMChannelNamesReducer,
  GetSMChannelStreams: GetSMChannelStreamsReducer,
  GetSMTasks: GetSMTasksReducer,
  GetStationChannelNames: GetStationChannelNamesReducer,
  GetStationPreviews: GetStationPreviewsReducer,
  GetStreamGroup: GetStreamGroupReducer,
  GetStreamGroupProfiles: GetStreamGroupProfilesReducer,
  GetStreamGroups: GetStreamGroupsReducer,
  GetStreamGroupSMChannels: GetStreamGroupSMChannelsReducer,
  GetStreamingStatisticsForChannel: GetStreamingStatisticsForChannelReducer,
  GetStreamStreamingStatistics: GetStreamStreamingStatisticsReducer,
  GetSystemStatus: GetSystemStatusReducer,
  GetTaskIsRunning: GetTaskIsRunningReducer,
  GetVideoInfoFromId: GetVideoInfoFromIdReducer,
  GetVideoProfiles: GetVideoProfilesReducer,
  isTrue: persistReducer(isTrueConfig, isTrue),
  loading: loading,
  messages: messages,
  queryAdditionalFilters: persistReducer(queryAdditionalFiltersConfig, queryAdditionalFilters),
  queryFilter: persistReducer(queryFilterConfig, queryFilter),
  selectAll: persistReducer(selectAllConfig, selectAll),
  selectedCountry: persistReducer(selectedCountryConfig, selectedCountry),
  selectedItems: persistReducer(selectedItemsConfig, selectedItems),
  selectedPostalCode: persistReducer(selectedPostalCodeConfig, selectedPostalCode),
  selectedSMChannel: persistReducer(selectedSMChannelConfig, selectedSMChannel),
  selectedSMStream: persistReducer(selectedSMStreamConfig, selectedSMStream),
  selectedSMStreams: persistReducer(selectedSMStreamsConfig, selectedSMStreams),
  selectedStreamGroup: persistReducer(selectedStreamGroupConfig, selectedStreamGroup),
  showHidden: persistReducer(showHiddenConfig, showHidden),
  showSelections: persistReducer(showSelectionsConfig, showSelections),
  sortInfo: persistReducer(sortInfoConfig, sortInfo),
  stringValue: persistReducer(stringValueConfig, stringValue),
  updateSettingRequest: updateSettingRequest,
});

