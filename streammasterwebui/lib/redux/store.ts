import { configureStore, type Action, type ThunkAction } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query/react';
import { combineReducers } from 'redux';

import { enhancedApiChannelGroups } from '@/lib/smAPI/ChannelGroups/ChannelGroupsEnhancedAPI';
import { enhancedApiEpgFiles } from '@/lib/smAPI/EpgFiles/EpgFilesEnhancedAPI';
import { enhancedApiM3UFiles } from '@/lib/smAPI/M3UFiles/M3UFilesEnhancedAPI';
import { enhancedApiProgrammes } from '@/lib/smAPI/Programmes/ProgrammesEnhancedAPI';
import { enhancedApiSchedulesDirect } from '@/lib/smAPI/SchedulesDirect/SchedulesDirectEnhancedAPI';
import { enhancedApiSettings } from '@/lib/smAPI/Settings/SettingsEnhancedAPI';
import { enhancedApiStreamGroupChannelGroup } from '@/lib/smAPI/StreamGroupChannelGroup/StreamGroupChannelGroupEnhancedAPI';
import { enhancedApiStreamGroupVideoStreams } from '@/lib/smAPI/StreamGroupVideoStreams/StreamGroupVideoStreamsEnhancedAPI';
import { enhancedApiStreamGroups } from '@/lib/smAPI/StreamGroups/StreamGroupsEnhancedAPI';
import { enhancedApiVideoStreamLinks } from '@/lib/smAPI/VideoStreamLinks/VideoStreamLinksEnhancedAPI';
import { enhancedApiVideoStreams } from '@/lib/smAPI/VideoStreams/VideoStreamsEnhancedAPI';
import { persistStore } from 'redux-persist';
import storage from 'redux-persist/lib/storage';

import { enhancedApiVideoStreamLinksLocal } from '@/lib/smAPILocal/VideoStreamLinksEnhancedAPILocal';

import channelGroupToRemoveSliceReducer from '@/lib/redux/slices/channelGroupToRemoveSlice';
import queryAdditionalFiltersReducer from '@/lib/redux/slices/queryAdditionalFiltersSlice';
import queryFilterReducer from '@/lib/redux/slices/queryFilterSlice';
import selectAllSliceReducer from '@/lib/redux/slices/selectAllSlice';
import selectedChannelGroupsSliceReducer from '@/lib/redux/slices/selectedChannelGroupsSlice';
import selectedItemsSliceReducer from '@/lib/redux/slices/selectedItemsSlice';
import selectedStreamGroupSliceReducer from '@/lib/redux/slices/selectedStreamGroupSlice';
import selectedVideoStreamsSliceReducer from '@/lib/redux/slices/selectedVideoStreamsSlice';
import showHiddenSliceReducer from '@/lib/redux/slices/showHiddenSlice';
import sortInfoSliceReducer from '@/lib/redux/slices/sortInfoSlice';
import { enhancedApiVideoStreamsGetAllStatisticsLocal } from '@/lib/smAPILocal/enhancedApiVideoStreamsGetAllStatisticsLocal';
import appInfoSliceReducer from './slices/appInfoSlice';
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

// const selectedChannelGroupsConfig = {
//   key: 'selectedChannelGroups',
//   storage,
// };

const selectedItemsGroupsConfig = {
  key: 'selectedItems',
  storage,
};

const selectedStreamGroupConfig = {
  key: 'selectedStreamGroup',
  storage,
};

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
//   selectAll: persistReducer(selectAllConfig, selectAllSliceReducer),
//   selectedChannelGroups: persistReducer(selectedItemsGroupsConfig, selectedChannelGroupsSliceReducer),
//   selectedItems: selectedItemsSliceReducer,
//   selectedStreamGroup: persistReducer(selectedStreamGroupConfig, selectedStreamGroupSliceReducer),
//   selectedVideoStreams:persistReducer(selectedVideoStreamsConfig, selectedVideoStreamsSliceReducer),
//   showHidden: persistReducer(showHiddenConfig, showHiddenSliceReducer),
//   sortInfo: persistReducer(sortInfoConfig, sortInfoSliceReducer),
// });

const rootReducer = combineReducers({
  appInfo:appInfoSliceReducer,
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
  channelGroupToRemove:channelGroupToRemoveSliceReducer,
  queryAdditionalFilters: queryAdditionalFiltersReducer,
  queryFilter: queryFilterReducer,
  selectAll:  selectAllSliceReducer,
  selectedChannelGroups: selectedChannelGroupsSliceReducer,
  selectedItems: selectedItemsSliceReducer,
  selectedStreamGroup: selectedStreamGroupSliceReducer,
  selectedVideoStreams: selectedVideoStreamsSliceReducer,
  showHidden: showHiddenSliceReducer,
  sortInfo: sortInfoSliceReducer,
});

export const store = configureStore({
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
      enhancedApiVideoStreamsGetAllStatisticsLocal.middleware,
    ),
  reducer: rootReducer,
});

export const persistor = persistStore(store);


// instead of defining the reducers in the reducer field of configureStore, combine them here:

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof rootReducer>;
export type AppThunk<ReturnType = void> = ThunkAction<
ReturnType,
RootState,
unknown,
Action<string>
>;

setupListeners(store.dispatch);

