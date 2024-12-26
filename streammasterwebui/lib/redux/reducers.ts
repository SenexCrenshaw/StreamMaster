import { combineReducers } from 'redux';
import currentSettingRequest from '@lib/redux/hooks/currentSettingRequest';
import filters from '@lib/redux/hooks/filters';
import GetAvailableCountriesReducer from '@lib/smAPI/SchedulesDirect/GetAvailableCountriesSlice';
import GetChannelGroupsReducer from '@lib/smAPI/ChannelGroups/GetChannelGroupsSlice';
import GetChannelGroupsFromSMChannelsReducer from '@lib/smAPI/ChannelGroups/GetChannelGroupsFromSMChannelsSlice';
import GetChannelMetricsReducer from '@lib/smAPI/Statistics/GetChannelMetricsSlice';
import GetCommandProfilesReducer from '@lib/smAPI/Profiles/GetCommandProfilesSlice';
import GetCustomLogosReducer from '@lib/smAPI/Logos/GetCustomLogosSlice';
import GetCustomPlayListReducer from '@lib/smAPI/Custom/GetCustomPlayListSlice';
import GetCustomPlayListsReducer from '@lib/smAPI/Custom/GetCustomPlayListsSlice';
import GetDownloadServiceStatusReducer from '@lib/smAPI/General/GetDownloadServiceStatusSlice';
import GetEPGColorsReducer from '@lib/smAPI/EPG/GetEPGColorsSlice';
import GetEPGFileNamesReducer from '@lib/smAPI/EPGFiles/GetEPGFileNamesSlice';
import GetEPGFilePreviewByIdReducer from '@lib/smAPI/EPGFiles/GetEPGFilePreviewByIdSlice';
import GetEPGFilesReducer from '@lib/smAPI/EPGFiles/GetEPGFilesSlice';
import GetEPGNextEPGNumberReducer from '@lib/smAPI/EPGFiles/GetEPGNextEPGNumberSlice';
import GetHeadendsByCountryPostalReducer from '@lib/smAPI/SchedulesDirect/GetHeadendsByCountryPostalSlice';
import GetHeadendsToViewReducer from '@lib/smAPI/SchedulesDirect/GetHeadendsToViewSlice';
import GetIntroPlayListsReducer from '@lib/smAPI/Custom/GetIntroPlayListsSlice';
import GetIsSystemReadyReducer from '@lib/smAPI/General/GetIsSystemReadySlice';
import GetLineupPreviewChannelReducer from '@lib/smAPI/SchedulesDirect/GetLineupPreviewChannelSlice';
import GetLogContentsReducer from '@lib/smAPI/Logs/GetLogContentsSlice';
import GetLogNamesReducer from '@lib/smAPI/Logs/GetLogNamesSlice';
import GetLogoReducer from '@lib/smAPI/Logos/GetLogoSlice';
import GetLogoForChannelReducer from '@lib/smAPI/Logos/GetLogoForChannelSlice';
import GetLogosReducer from '@lib/smAPI/Logos/GetLogosSlice';
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
import GetSMChannelChannelsReducer from '@lib/smAPI/SMChannelChannelLinks/GetSMChannelChannelsSlice';
import GetSMChannelNamesReducer from '@lib/smAPI/SMChannels/GetSMChannelNamesSlice';
import GetSMChannelStreamsReducer from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsSlice';
import GetSMTasksReducer from '@lib/smAPI/SMTasks/GetSMTasksSlice';
import GetStationChannelNamesReducer from '@lib/smAPI/SchedulesDirect/GetStationChannelNamesSlice';
import GetStationPreviewsReducer from '@lib/smAPI/SchedulesDirect/GetStationPreviewsSlice';
import GetStreamConnectionMetricReducer from '@lib/smAPI/Statistics/GetStreamConnectionMetricSlice';
import GetStreamConnectionMetricsReducer from '@lib/smAPI/Statistics/GetStreamConnectionMetricsSlice';
import GetStreamGroupReducer from '@lib/smAPI/StreamGroups/GetStreamGroupSlice';
import GetStreamGroupProfilesReducer from '@lib/smAPI/StreamGroups/GetStreamGroupProfilesSlice';
import GetStreamGroupsReducer from '@lib/smAPI/StreamGroups/GetStreamGroupsSlice';
import GetStreamGroupSMChannelsReducer from '@lib/smAPI/StreamGroupSMChannelLinks/GetStreamGroupSMChannelsSlice';
import GetSubScribedHeadendsReducer from '@lib/smAPI/SchedulesDirect/GetSubScribedHeadendsSlice';
import GetSubscribedLineupsReducer from '@lib/smAPI/SchedulesDirect/GetSubscribedLineupsSlice';
import GetSystemStatusReducer from '@lib/smAPI/General/GetSystemStatusSlice';
import GetTaskIsRunningReducer from '@lib/smAPI/General/GetTaskIsRunningSlice';
import GetVideoInfoReducer from '@lib/smAPI/Statistics/GetVideoInfoSlice';
import GetVideoInfosReducer from '@lib/smAPI/Statistics/GetVideoInfosSlice';
import GetVideoStreamNamesAndUrlsReducer from '@lib/smAPI/SMChannels/GetVideoStreamNamesAndUrlsSlice';
import GetVsReducer from '@lib/smAPI/Vs/GetVsSlice';
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
import showSelected from '@lib/redux/hooks/showSelected';
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
const showSelectedConfig = {
  key: 'showSelected',
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
  GetChannelMetrics: GetChannelMetricsReducer,
  GetCommandProfiles: GetCommandProfilesReducer,
  GetCustomLogos: GetCustomLogosReducer,
  GetCustomPlayList: GetCustomPlayListReducer,
  GetCustomPlayLists: GetCustomPlayListsReducer,
  GetDownloadServiceStatus: GetDownloadServiceStatusReducer,
  GetEPGColors: GetEPGColorsReducer,
  GetEPGFileNames: GetEPGFileNamesReducer,
  GetEPGFilePreviewById: GetEPGFilePreviewByIdReducer,
  GetEPGFiles: GetEPGFilesReducer,
  GetEPGNextEPGNumber: GetEPGNextEPGNumberReducer,
  GetHeadendsByCountryPostal: GetHeadendsByCountryPostalReducer,
  GetHeadendsToView: GetHeadendsToViewReducer,
  GetIntroPlayLists: GetIntroPlayListsReducer,
  GetIsSystemReady: GetIsSystemReadyReducer,
  GetLineupPreviewChannel: GetLineupPreviewChannelReducer,
  GetLogContents: GetLogContentsReducer,
  GetLogNames: GetLogNamesReducer,
  GetLogo: GetLogoReducer,
  GetLogoForChannel: GetLogoForChannelReducer,
  GetLogos: GetLogosReducer,
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
  GetSMChannelChannels: GetSMChannelChannelsReducer,
  GetSMChannelNames: GetSMChannelNamesReducer,
  GetSMChannelStreams: GetSMChannelStreamsReducer,
  GetSMTasks: GetSMTasksReducer,
  GetStationChannelNames: GetStationChannelNamesReducer,
  GetStationPreviews: GetStationPreviewsReducer,
  GetStreamConnectionMetric: GetStreamConnectionMetricReducer,
  GetStreamConnectionMetrics: GetStreamConnectionMetricsReducer,
  GetStreamGroup: GetStreamGroupReducer,
  GetStreamGroupProfiles: GetStreamGroupProfilesReducer,
  GetStreamGroups: GetStreamGroupsReducer,
  GetStreamGroupSMChannels: GetStreamGroupSMChannelsReducer,
  GetSubScribedHeadends: GetSubScribedHeadendsReducer,
  GetSubscribedLineups: GetSubscribedLineupsReducer,
  GetSystemStatus: GetSystemStatusReducer,
  GetTaskIsRunning: GetTaskIsRunningReducer,
  GetVideoInfo: GetVideoInfoReducer,
  GetVideoInfos: GetVideoInfosReducer,
  GetVideoStreamNamesAndUrls: GetVideoStreamNamesAndUrlsReducer,
  GetVs: GetVsReducer,
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
  showSelected: persistReducer(showSelectedConfig, showSelected),
  showSelections: persistReducer(showSelectionsConfig, showSelections),
  sortInfo: persistReducer(sortInfoConfig, sortInfo),
  stringValue: persistReducer(stringValueConfig, stringValue),
  updateSettingRequest: updateSettingRequest,
});

