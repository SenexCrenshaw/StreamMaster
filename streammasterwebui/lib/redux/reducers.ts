import { combineReducers } from 'redux';
import GetChannelGroups from '@lib/smAPI/ChannelGroups/GetChannelGroupsSlice';
import GetEPGColors from '@lib/smAPI/EPG/GetEPGColorsSlice';
import GetEPGFilePreviewById from '@lib/smAPI/EPGFiles/GetEPGFilePreviewByIdSlice';
import GetEPGFiles from '@lib/smAPI/EPGFiles/GetEPGFilesSlice';
import GetEPGNextEPGNumber from '@lib/smAPI/EPGFiles/GetEPGNextEPGNumberSlice';
import GetIcons from '@lib/smAPI/Icons/GetIconsSlice';
import GetIsSystemReady from '@lib/smAPI/Settings/GetIsSystemReadySlice';
import GetPagedChannelGroups from '@lib/smAPI/ChannelGroups/GetPagedChannelGroupsSlice';
import GetPagedEPGFiles from '@lib/smAPI/EPGFiles/GetPagedEPGFilesSlice';
import GetPagedM3UFiles from '@lib/smAPI/M3UFiles/GetPagedM3UFilesSlice';
import GetPagedSMChannels from '@lib/smAPI/SMChannels/GetPagedSMChannelsSlice';
import GetPagedSMStreams from '@lib/smAPI/SMStreams/GetPagedSMStreamsSlice';
import GetPagedStreamGroups from '@lib/smAPI/StreamGroups/GetPagedStreamGroupsSlice';
import GetSettings from '@lib/smAPI/Settings/GetSettingsSlice';
import GetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsSlice';
import GetStationChannelNames from '@lib/smAPI/SchedulesDirect/GetStationChannelNamesSlice';
import GetStreamGroups from '@lib/smAPI/StreamGroups/GetStreamGroupsSlice';
import GetStreamGroupSMChannels from '@lib/smAPI/StreamGroupSMChannelLinks/GetStreamGroupSMChannelsSlice';
import GetSystemStatus from '@lib/smAPI/Settings/GetSystemStatusSlice';
import selectAll from '@lib/redux/slices/selectAllSlice';
import selectedCountry from '@lib/redux/slices/selectedCountrySlice';
import selectedItems from '@lib/redux/slices/selectedItemsSlice';
import selectedPostalCode from '@lib/redux/slices/selectedPostalCodeSlice';
import selectedSMStreams from '@lib/redux/slices/selectedSMStreamsSlice';
import selectedStreamGroup from '@lib/redux/slices/selectedStreamGroupSlice';
import showHidden from '@lib/redux/slices/showHiddenSlice';
import showSelections from '@lib/redux/slices/showSelectionsSlice';
import SMChannelReducer from '@lib/redux/slices/selectedSMChannel';
import SMMessagesReducer from '@lib/redux/slices/messagesSlice';
import SMStreamReducer from '@lib/redux/slices/selectedSMStream';
import sortInfo from '@lib/redux/slices/sortInfoSlice';
import queryFilterReducer from '@lib/redux/slices/queryFilterSlice';
import { persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';

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
  GetPagedChannelGroups: GetPagedChannelGroups,
  GetPagedEPGFiles: GetPagedEPGFiles,
  GetPagedM3UFiles: GetPagedM3UFiles,
  GetPagedSMChannels: GetPagedSMChannels,
  GetPagedSMStreams: GetPagedSMStreams,
  GetPagedStreamGroups: GetPagedStreamGroups,
  GetSettings: GetSettings,
  GetSMChannelStreams: GetSMChannelStreams,
  GetStationChannelNames: GetStationChannelNames,
  GetStreamGroups: GetStreamGroups,
  GetStreamGroupSMChannels: GetStreamGroupSMChannels,
  GetSystemStatus: GetSystemStatus,
  messages: SMMessagesReducer,
  queryAdditionalFilters: queryFilterReducer,
  queryFilter: queryFilterReducer,
  selectAll: persistReducer(selectAllConfig, selectAll),
  selectedCountry: persistReducer(selectedCountryConfig, selectedCountry),
  selectedItems: persistReducer(selectedItemsConfig, selectedItems),
  selectedPostalCode: persistReducer(selectedPostalCodeConfig, selectedPostalCode),
  selectedSMStreams: persistReducer(selectedSMStreamsConfig, selectedSMStreams),
  selectedStreamGroup: persistReducer(selectedStreamGroupConfig, selectedStreamGroup),
  showHidden: persistReducer(showHiddenConfig, showHidden),
  showSelections: persistReducer(showSelectionsConfig, showSelections),
  SMChannelReducer: SMChannelReducer,
  SMStreamReducer: SMStreamReducer,
  sortInfo: persistReducer(sortInfoConfig, sortInfo),
});

