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
import GetChannelGroups from '@lib/smAPI/ChannelGroups/GetChannelGroupsSlice';
import GetPagedChannelGroups from '@lib/smAPI/ChannelGroups/GetPagedChannelGroupsSlice';
import GetEPGColors from '@lib/smAPI/EPG/GetEPGColorsSlice';
import GetEPGFilePreviewById from '@lib/smAPI/EPGFiles/GetEPGFilePreviewByIdSlice';
import GetEPGFiles from '@lib/smAPI/EPGFiles/GetEPGFilesSlice';
import GetEPGNextEPGNumber from '@lib/smAPI/EPGFiles/GetEPGNextEPGNumberSlice';
import GetPagedEPGFiles from '@lib/smAPI/EPGFiles/GetPagedEPGFilesSlice';
import GetIcons from '@lib/smAPI/Icons/GetIconsSlice';
import GetM3UFileNames from '@lib/smAPI/M3UFiles/GetM3UFileNamesSlice';
import GetPagedM3UFiles from '@lib/smAPI/M3UFiles/GetPagedM3UFilesSlice';
import GetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsSlice';
import GetPagedSMChannels from '@lib/smAPI/SMChannels/GetPagedSMChannelsSlice';
import GetSMChannelNames from '@lib/smAPI/SMChannels/GetSMChannelNamesSlice';
import GetSMChannel from '@lib/smAPI/SMChannels/GetSMChannelSlice';
import GetPagedSMStreams from '@lib/smAPI/SMStreams/GetPagedSMStreamsSlice';
import GetStationChannelNames from '@lib/smAPI/SchedulesDirect/GetStationChannelNamesSlice';
import GetIsSystemReady from '@lib/smAPI/Settings/GetIsSystemReadySlice';
import GetSettings from '@lib/smAPI/Settings/GetSettingsSlice';
import GetSystemStatus from '@lib/smAPI/Settings/GetSystemStatusSlice';
import GetStreamGroupSMChannels from '@lib/smAPI/StreamGroupSMChannelLinks/GetStreamGroupSMChannelsSlice';
import GetPagedStreamGroups from '@lib/smAPI/StreamGroups/GetPagedStreamGroupsSlice';
import GetStreamGroups from '@lib/smAPI/StreamGroups/GetStreamGroupsSlice';
import { combineReducers } from 'redux';
import { persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';

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
export const rootReducer = combineReducers({
  GetChannelGroups: GetChannelGroups,
  GetEPGColors: GetEPGColors,
  GetEPGFilePreviewById: GetEPGFilePreviewById,
  GetEPGFiles: GetEPGFiles,
  GetEPGNextEPGNumber: GetEPGNextEPGNumber,
  GetIcons: GetIcons,
  GetIsSystemReady: GetIsSystemReady,
  GetM3UFileNames: GetM3UFileNames,
  GetPagedChannelGroups: GetPagedChannelGroups,
  GetPagedEPGFiles: GetPagedEPGFiles,
  GetPagedM3UFiles: GetPagedM3UFiles,
  GetPagedSMChannels: GetPagedSMChannels,
  GetPagedSMStreams: GetPagedSMStreams,
  GetPagedStreamGroups: GetPagedStreamGroups,
  GetSettings: GetSettings,
  GetSMChannel: GetSMChannel,
  GetSMChannelNames: GetSMChannelNames,
  GetSMChannelStreams: GetSMChannelStreams,
  GetStationChannelNames: GetStationChannelNames,
  GetStreamGroups: GetStreamGroups,
  GetStreamGroupSMChannels: GetStreamGroupSMChannels,
  GetSystemStatus: GetSystemStatus,
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
  sortInfo: persistReducer(sortInfoConfig, sortInfo)
});
