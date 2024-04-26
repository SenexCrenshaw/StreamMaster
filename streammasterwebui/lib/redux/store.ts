import { configureStore, type Action, type ThunkAction } from '@reduxjs/toolkit';
import { combineReducers } from 'redux';

import channelGroupToRemoveSliceReducer from '@lib/redux/slices/channelGroupToRemoveSlice';
import queryAdditionalFiltersReducer from '@lib/redux/slices/queryAdditionalFiltersSlice';
import queryFilterReducer from '@lib/redux/slices/queryFilterSlice';
import selectAllSliceReducer from '@lib/redux/slices/selectAllSlice';
import selectedCountrySlice from '@lib/redux/slices/selectedCountrySlice';
import selectCurrentSettingDtoReducer from '@lib/redux/slices/selectedCurrentSettingDto';
import selectedItemsSliceReducer from '@lib/redux/slices/selectedItemsSlice';
import selectedPostalCodeSlice from '@lib/redux/slices/selectedPostalCodeSlice';
import SMChannelReducer from '@lib/redux/slices/selectedSMChannel';
import SMStreamReducer from '@lib/redux/slices/selectedSMStream';
import selectSMStreamsReducer from '@lib/redux/slices/selectedSMStreams';
import selectedStreamGroupSliceReducer from '@lib/redux/slices/selectedStreamGroupSlice';
import selectUpdateSettingRequestReducer from '@lib/redux/slices/selectedUpdateSettingRequestSlice';
import selectedVideoStreamsSliceReducer from '@lib/redux/slices/selectedVideoStreamsSlice';
import showHiddenSliceReducer from '@lib/redux/slices/showHiddenSlice';
import showSelectionsSliceReducer from '@lib/redux/slices/showSelectionsSlice';
import sortInfoSliceReducer from '@lib/redux/slices/sortInfoSlice';
import GetPagedChannelGroups from '@lib/smAPI/ChannelGroups/GetPagedChannelGroupsSlice';
import GetIcons from '@lib/smAPI/Icons/GetIconsSlice';
import GetPagedM3UFiles from '@lib/smAPI/M3UFiles/GetPagedM3UFilesSlice';
import GetPagedSMChannels from '@lib/smAPI/SMChannels/GetPagedSMChannelsSlice';
import GetPagedSMStreams from '@lib/smAPI/SMStreams/GetPagedSMStreamsSlice';
import GetStationChannelNames from '@lib/smAPI/SchedulesDirect/GetStationChannelNamesSlice';
import GetIsSystemReady from '@lib/smAPI/Settings/GetIsSystemReadySlice';
import GetSettings from '@lib/smAPI/Settings/GetSettingsSlice';
import GetSystemStatus from '@lib/smAPI/Settings/GetSystemStatusSlice';
import GetStreamGroups from '@lib/smAPI/StreamGroups/GetStreamGroupsSlice';

import GetEPGColorsSlice from '@lib/smAPI/EPG/GetEPGColorsSlice';
import GetEPGFilePreviewById from '@lib/smAPI/EPGFiles/GetEPGFilePreviewByIdSlice';
import GetEPGFiles from '@lib/smAPI/EPGFiles/GetEPGFilesSlice';
import GetEPGNextEPGNumber from '@lib/smAPI/EPGFiles/GetEPGNextEPGNumberSlice';
import GetPagedEPGFiles from '@lib/smAPI/EPGFiles/GetPagedEPGFilesSlice';
import StreamGroupSMChannelLinks from '@lib/smAPI/StreamGroupSMChannelLinks/GetStreamGroupSMChannelsSlice';

import GetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/GetSMChannelStreamsSlice';
import GetPagedStreamGroups from '@lib/smAPI/StreamGroups/GetPagedStreamGroupsSlice';

import anySlice from '@lib/redux/slices/anySlice';

import SMMessagesReducer from '@lib/redux/slices/messagesSlice';

import { persistReducer, persistStore } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import appInfoSliceReducer from './slices/appInfoSlice';

const selectAllConfig = {
  key: 'selectAll',
  storage
};

const sortInfoConfig = {
  key: 'sortInfo',
  storage
};

const showHiddenConfig = {
  key: 'showHidden',
  storage
};

const selectedVideoStreamsConfig = {
  key: 'selectedVideoStreams',
  storage
};

const selectSMStreamsConfig = {
  key: 'selectedVSMStreams',
  storage
};

const showSelectionsConfig = {
  key: 'showSelections',
  storage
};

// const selectedItemsGroupsConfig = {
//   key: 'selectedItems',
//   storage
// };

const selectedStreamGroupConfig = {
  key: 'selectedStreamGroup',
  storage
};

const selectedCountryConfig = {
  key: 'selectedCountry',
  storage
};

const selectedPostalCodeConfig = {
  key: 'selectedPostalCode',
  storage
};

const currentSettingDtoSliceConfig = {
  key: 'currentSettingDtoSlice',
  storage
};

const selectUpdateSettingRequestSliceConfig = {
  key: 'selectUpdateSettingRequestSlice',
  storage
};

const selectedItemsConfig = {
  key: 'selectedItems',
  storage
};

const rootReducer = combineReducers({
  appInfo: appInfoSliceReducer,
  any: anySlice,
  channelGroupToRemove: channelGroupToRemoveSliceReducer,
  queryAdditionalFilters: queryAdditionalFiltersReducer,
  queryFilter: queryFilterReducer,
  GetPagedStreamGroups: GetPagedStreamGroups,
  GetPagedChannelGroups: GetPagedChannelGroups,
  GetStreamGroupSMChannels: StreamGroupSMChannelLinks,
  GetPagedM3UFiles: GetPagedM3UFiles,
  GetPagedSMChannels: GetPagedSMChannels,
  GetPagedSMStreams: GetPagedSMStreams,
  GetSettings: GetSettings,
  GetIsSystemReady: GetIsSystemReady,
  GetSystemStatus: GetSystemStatus,
  GetIcons: GetIcons,
  GetSMChannelStreams: GetSMChannelStreams,
  SMChannelReducer: SMChannelReducer,
  SMStreamReducer: SMStreamReducer,
  GetEPGColors: GetEPGColorsSlice,
  GetEPGFiles: GetEPGFiles,
  GetStreamGroups: GetStreamGroups,
  GetPagedEPGFiles: GetPagedEPGFiles,
  GetEPGFilePreviewById: GetEPGFilePreviewById,
  GetEPGNextEPGNumber: GetEPGNextEPGNumber,
  GetStationChannelNames: GetStationChannelNames,
  selectUpdateSettingRequest: persistReducer(selectUpdateSettingRequestSliceConfig, selectUpdateSettingRequestReducer),
  selectCurrentSettingDto: persistReducer(currentSettingDtoSliceConfig, selectCurrentSettingDtoReducer),
  selectedPostalCode: persistReducer(selectedPostalCodeConfig, selectedPostalCodeSlice),
  selectAll: persistReducer(selectAllConfig, selectAllSliceReducer),
  selectedCountry: persistReducer(selectedCountryConfig, selectedCountrySlice),
  selectedItems: persistReducer(selectedItemsConfig, selectedItemsSliceReducer),
  selectedStreamGroup: persistReducer(selectedStreamGroupConfig, selectedStreamGroupSliceReducer),
  selectedVideoStreams: persistReducer(selectedVideoStreamsConfig, selectedVideoStreamsSliceReducer),
  showHidden: persistReducer(showHiddenConfig, showHiddenSliceReducer),
  showSelections: persistReducer(showSelectionsConfig, showSelectionsSliceReducer),
  sortInfo: persistReducer(sortInfoConfig, sortInfoSliceReducer),
  selectSMStreams: persistReducer(selectSMStreamsConfig, selectSMStreamsReducer),
  messages: SMMessagesReducer
});

export type RootState = ReturnType<typeof rootReducer>;

const store = configureStore({
  devTools: process.env.NODE_ENV !== 'production',
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      immutableCheck: false,
      serializableCheck: false
    }).concat(),
  reducer: rootReducer
});

export type AppDispatch = typeof store.dispatch;

export type AppThunk<ReturnType = void> = ThunkAction<ReturnType, RootState, unknown, Action<string>>;

export const persistor = persistStore(store);

export default store;
