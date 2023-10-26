import { Store, configureStore, type Action, type ThunkAction } from '@reduxjs/toolkit';
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
import selectedChannelGroupsSliceReducer from '@lib/redux/slices/selectedChannelGroupsSlice';
import selectedItemsSliceReducer from '@lib/redux/slices/selectedItemsSlice';
import selectedStreamGroupSliceReducer from '@lib/redux/slices/selectedStreamGroupSlice';
import selectedVideoStreamsSliceReducer from '@lib/redux/slices/selectedVideoStreamsSlice';
import showHiddenSliceReducer from '@lib/redux/slices/showHiddenSlice';
import showSelectionsSliceReducer from '@lib/redux/slices/showSelectionsSlice';
import sortInfoSliceReducer from '@lib/redux/slices/sortInfoSlice';

import { enhancedApiVideoStreamsGetAllStatisticsLocal } from '@lib/smAPILocal/enhancedApiVideoStreamsGetAllStatisticsLocal';
import { useMemo } from 'react';
import { persistReducer } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import appInfoSliceReducer from './slices/appInfoSlice';

let store: any;

const selectAllConfig = {
  key: 'selectAll',
  storage,
};

const sortInfoConfig = {
  key: 'sortInfo',
  storage,
};

const showHiddenConfig = {
  key: 'showHidden',
  storage,
};

const selectedVideoStreamsConfig = {
  key: 'selectedVideoStreams',
  storage,
};

const showSelectionsConfig = {
  key: 'showSelections',
  storage,
};

const selectedItemsGroupsConfig = {
  key: 'selectedItems',
  storage,
};

const selectedStreamGroupConfig = {
  key: 'selectedStreamGroup',
  storage,
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
  selectAll: persistReducer(selectAllConfig, selectAllSliceReducer),
  selectedChannelGroups: persistReducer(selectedItemsGroupsConfig, selectedChannelGroupsSliceReducer),
  selectedItems: selectedItemsSliceReducer,
  selectedStreamGroup: persistReducer(selectedStreamGroupConfig, selectedStreamGroupSliceReducer),
  selectedVideoStreams: persistReducer(selectedVideoStreamsConfig, selectedVideoStreamsSliceReducer),
  showHidden: persistReducer(showHiddenConfig, showHiddenSliceReducer),
  showSelections: persistReducer(showSelectionsConfig, showSelectionsSliceReducer),
  sortInfo: persistReducer(sortInfoConfig, sortInfoSliceReducer),
});

export type RootState = ReturnType<typeof rootReducer>;

// const rootReducer = combineReducers({
//   appInfo:appInfoSliceReducer,
//   [enhancedApiChannelGroups.reducerPath]: enhancedApiChannelGroups.reducer,
//   [enhancedApiEpgFiles.reducerPath]: enhancedApiEpgFiles.reducer,
//   [enhancedApiM3UFiles.reducerPath]: enhancedApiM3UFiles.reducer,
//   [enhancedApiProgrammes.reducerPath]: enhancedApiProgrammes.reducer,
//   [enhancedApiSchedulesDirect.reducerPath]: enhancedApiSchedulesDirect.reducer,
//   [enhancedApiSettings.reducerPath]: enhancedApiSettings.reducer,
//   [enhancedApiStreamGroupChannelGroup.reducerPath]: enhancedApiStreamGroupChannelGroup.reducer,
//   [enhancedApiStreamGroupVideoStreams.reducerPath]: enhancedApiStreamGroupVideoStreams.reducer,
//   [enhancedApiStreamGroups.reducerPath]: enhancedApiStreamGroups.reducer,
//   [enhancedApiVideoStreamLinks.reducerPath]: enhancedApiVideoStreamLinks.reducer,
//   [enhancedApiVideoStreams.reducerPath]: enhancedApiVideoStreams.reducer,
//   [enhancedApiVideoStreamLinksLocal.reducerPath]: enhancedApiVideoStreamLinksLocal.reducer,
//   [enhancedApiVideoStreamsGetAllStatisticsLocal.reducerPath]: enhancedApiVideoStreamsGetAllStatisticsLocal.reducer,
//   channelGroupToRemove:channelGroupToRemoveSliceReducer,
//   queryAdditionalFilters: queryAdditionalFiltersReducer,
//   queryFilter: queryFilterReducer,
//   selectAll:  selectAllSliceReducer,
//   selectedChannelGroups: selectedChannelGroupsSliceReducer,
//   selectedItems: selectedItemsSliceReducer,
//   selectedStreamGroup: selectedStreamGroupSliceReducer,
//   selectedVideoStreams: selectedVideoStreamsSliceReducer,
//   showHidden: showHiddenSliceReducer,
//   sortInfo: sortInfoSliceReducer,
// });

function makeStore(): Store<RootState> {
  return configureStore({
    devTools: process.env.NODE_ENV !== 'production',
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware({
        immutableCheck: false,
        serializableCheck: false,
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
    reducer: rootReducer,
  });
}

export const initializeStore = (): Store<RootState> => {
  let _store = store ?? makeStore();

  // For SSG and SSR always create a new store
  if (typeof window === 'undefined') return _store;

  // Create the store once in the client
  if (!store) store = _store;

  return _store;
};

export type AppDispatch = typeof store.dispatch;
// setupListeners(store.dispatch);

export type AppThunk<ReturnType = void> = ThunkAction<ReturnType, RootState, unknown, Action<string>>;

export function useStore(): Store<RootState> {
  const store = useMemo(() => initializeStore(), []);
  return store;
}

export default makeStore;
