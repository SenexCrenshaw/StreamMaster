import { configureStore, type Action, type ThunkAction } from '@reduxjs/toolkit';
import { combineReducers } from 'redux';

import { enhancedApiChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsEnhancedAPI';
import { enhancedApiEpgFiles } from '@lib/smAPI/EpgFiles/EpgFilesEnhancedAPI';
import { enhancedApiM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesEnhancedAPI';
import { enhancedApiProgrammes } from '@lib/smAPI/Programmes/ProgrammesEnhancedAPI';
import { enhancedApiSchedulesDirect } from '@lib/smAPI/SchedulesDirect/SchedulesDirectEnhancedAPI';
import { enhancedApiSettings } from '@lib/smAPI/Settings/SettingsEnhancedAPI';
import { enhancedApiStreamGroupChannelGroup } from '@lib/smAPI/StreamGroupChannelGroup/StreamGroupChannelGroupEnhancedAPI';
import { enhancedApiStreamGroupVideoStreams } from '@lib/smAPI/StreamGroupVideoStreams/StreamGroupVideoStreamsEnhancedAPI';
import { enhancedApiStreamGroups } from '@lib/smAPI/StreamGroups/StreamGroupsEnhancedAPI';
import { enhancedApiVideoStreamLinks } from '@lib/smAPI/VideoStreamLinks/VideoStreamLinksEnhancedAPI';
import { enhancedApiVideoStreams } from '@lib/smAPI/VideoStreams/VideoStreamsEnhancedAPI';

import { enhancedApiVideoStreamLinksLocal } from '@lib/smAPILocal/VideoStreamLinksEnhancedAPILocal';

import channelGroupToRemoveSliceReducer from '@lib/redux/slices/channelGroupToRemoveSlice';
import queryAdditionalFiltersReducer from '@lib/redux/slices/queryAdditionalFiltersSlice';
import queryFilterReducer from '@lib/redux/slices/queryFilterSlice';
import selectAllSliceReducer from '@lib/redux/slices/selectAllSlice';
import selectedCountrySlice from '@lib/redux/slices/selectedCountrySlice';
import selectedItemsSliceReducer from '@lib/redux/slices/selectedItemsSlice';
import selectedPostalCodeSlice from '@lib/redux/slices/selectedPostalCodeSlice';
import selectedStreamGroupSliceReducer from '@lib/redux/slices/selectedStreamGroupSlice';
import selectedVideoStreamsSliceReducer from '@lib/redux/slices/selectedVideoStreamsSlice';
import showHiddenSliceReducer from '@lib/redux/slices/showHiddenSlice';
import showSelectionsSliceReducer from '@lib/redux/slices/showSelectionsSlice';
import sortInfoSliceReducer from '@lib/redux/slices/sortInfoSlice';

import { enhancedApiVideoStreamsGetAllStatisticsLocal } from '@lib/smAPILocal/enhancedApiVideoStreamsGetAllStatisticsLocal';
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

const showSelectionsConfig = {
  key: 'showSelections',
  storage
};

const selectedItemsGroupsConfig = {
  key: 'selectedItems',
  storage
};

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

const selectedItemsConfig = {
  key: 'selectedItems',
  storage
};

const rootReducer = combineReducers({
  appInfo: appInfoSliceReducer,
  [enhancedApiChannelGroups.reducerPath]: enhancedApiChannelGroups.reducer,
  [enhancedApiEpgFiles.reducerPath]: enhancedApiEpgFiles.reducer,
  [enhancedApiM3UFiles.reducerPath]: enhancedApiM3UFiles.reducer,
  [enhancedApiProgrammes.reducerPath]: enhancedApiProgrammes.reducer,
  [enhancedApiSchedulesDirect.reducerPath]: enhancedApiSchedulesDirect.reducer,
  [enhancedApiSettings.reducerPath]: enhancedApiSettings.reducer,
  [enhancedApiStreamGroupChannelGroup.reducerPath]: enhancedApiStreamGroupChannelGroup.reducer,
  [enhancedApiStreamGroupVideoStreams.reducerPath]: enhancedApiStreamGroupVideoStreams.reducer,
  [enhancedApiStreamGroups.reducerPath]: enhancedApiStreamGroups.reducer,
  [enhancedApiVideoStreamLinks.reducerPath]: enhancedApiVideoStreamLinks.reducer,
  [enhancedApiVideoStreams.reducerPath]: enhancedApiVideoStreams.reducer,
  [enhancedApiVideoStreamLinksLocal.reducerPath]: enhancedApiVideoStreamLinksLocal.reducer,
  [enhancedApiVideoStreamsGetAllStatisticsLocal.reducerPath]: enhancedApiVideoStreamsGetAllStatisticsLocal.reducer,
  channelGroupToRemove: channelGroupToRemoveSliceReducer,
  queryAdditionalFilters: queryAdditionalFiltersReducer,
  queryFilter: queryFilterReducer,
  selectedPostalCode: persistReducer(selectedPostalCodeConfig, selectedPostalCodeSlice),
  selectAll: persistReducer(selectAllConfig, selectAllSliceReducer),
  selectedCountry: persistReducer(selectedCountryConfig, selectedCountrySlice),
  selectedItems: persistReducer(selectedItemsConfig, selectedItemsSliceReducer),
  selectedStreamGroup: persistReducer(selectedStreamGroupConfig, selectedStreamGroupSliceReducer),
  selectedVideoStreams: persistReducer(selectedVideoStreamsConfig, selectedVideoStreamsSliceReducer),
  showHidden: persistReducer(showHiddenConfig, showHiddenSliceReducer),
  showSelections: persistReducer(showSelectionsConfig, showSelectionsSliceReducer),
  sortInfo: persistReducer(sortInfoConfig, sortInfoSliceReducer)
});

export type RootState = ReturnType<typeof rootReducer>;

const store = configureStore({
  devTools: process.env.NODE_ENV !== 'production',
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      immutableCheck: false,
      serializableCheck: false
    }).concat(
      enhancedApiChannelGroups.middleware,
      enhancedApiEpgFiles.middleware,
      enhancedApiM3UFiles.middleware,
      enhancedApiProgrammes.middleware,
      enhancedApiSchedulesDirect.middleware,
      enhancedApiSettings.middleware,
      enhancedApiStreamGroupChannelGroup.middleware,
      enhancedApiStreamGroups.middleware,
      enhancedApiStreamGroupVideoStreams.middleware,
      enhancedApiVideoStreamLinks.middleware,
      enhancedApiVideoStreams.middleware,
      enhancedApiVideoStreamLinksLocal.middleware,
      enhancedApiVideoStreamsGetAllStatisticsLocal.middleware
    ),
  reducer: rootReducer
});

export type AppDispatch = typeof store.dispatch;

export type AppThunk<ReturnType = void> = ThunkAction<ReturnType, RootState, unknown, Action<string>>;

export const persistor = persistStore(store);

export default store;
