import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query/react';
import { combineReducers } from 'redux';

import { persistReducer, persistStore } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import { enhancedApiChannelGroups } from '../smAPI/ChannelGroups/ChannelGroupsEnhancedAPI';
import { enhancedApiEpgFiles } from '../smAPI/EpgFiles/EpgFilesEnhancedAPI';
import { enhancedApiM3UFiles } from '../smAPI/M3UFiles/M3UFilesEnhancedAPI';
import { enhancedApiProgrammes } from '../smAPI/Programmes/ProgrammesEnhancedAPI';
import { enhancedApiSchedulesDirect } from '../smAPI/SchedulesDirect/SchedulesDirectEnhancedAPI';
import { enhancedApiSettings } from '../smAPI/Settings/SettingsEnhancedAPI';
import { enhancedApiStreamGroupChannelGroup } from '../smAPI/StreamGroupChannelGroup/StreamGroupChannelGroupEnhancedAPI';
import { enhancedApiStreamGroupVideoStreams } from '../smAPI/StreamGroupVideoStreams/StreamGroupVideoStreamsEnhancedAPI';
import { enhancedApiStreamGroups } from '../smAPI/StreamGroups/StreamGroupsEnhancedAPI';
import { enhancedApiVideoStreamLinks } from '../smAPI/VideoStreamLinks/VideoStreamLinksEnhancedAPI';
import { enhancedApiVideoStreams } from '../smAPI/VideoStreams/VideoStreamsEnhancedAPI';
import { enhancedApiVideoStreamsLocal } from '../smAPI/VideoStreams/enhancedApiVideoStreamsLocal';

import appInfoSliceReducer from './slices/appInfoSlice';
import channelGroupToRemoveSliceReducer from './slices/channelGroupToRemoveSlice';
import queryAdditionalFiltersReducer from './slices/queryAdditionalFiltersSlice';
import queryFilterReducer from './slices/queryFilterSlice';
import selectAllSliceReducer from './slices/selectAllSlice';
import selectedChannelGroupsSliceReducer from './slices/selectedChannelGroupsSlice';
import selectedStreamGroupSliceeReducer from './slices/selectedStreamGroupSlice';
import selectedVideoStreamsSliceReducer from './slices/selectedVideoStreamsSlice';
import showHiddenSliceReducer from './slices/showHiddenSlice';
import sortInfoSliceReducer from './slices/sortInfoSlice';

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

const selectedChannelGroupsConfig = {
  key: 'selectedChannelGroups',
  storage,
};


const rootReducer = combineReducers({
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
  [enhancedApiVideoStreamsLocal.reducerPath]: enhancedApiVideoStreamsLocal.reducer,
  appInfo:appInfoSliceReducer,
  channelGroupToRemove:channelGroupToRemoveSliceReducer,
  queryAdditionalFilters: queryAdditionalFiltersReducer,
  queryFilter: queryFilterReducer,
  selectAll: persistReducer(selectAllConfig, selectAllSliceReducer),
  selectedChannelGroups: persistReducer(selectedChannelGroupsConfig, selectedChannelGroupsSliceReducer),
  selectedStreamGroup:  selectedStreamGroupSliceeReducer,
  selectedVideoStreams:persistReducer(selectedVideoStreamsConfig, selectedVideoStreamsSliceReducer),
  showHidden: persistReducer(showHiddenConfig, showHiddenSliceReducer),
  sortInfo: persistReducer(sortInfoConfig, sortInfoSliceReducer),
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
      enhancedApiVideoStreamsLocal.middleware,
    ),
  reducer: rootReducer,
});

export const persistor = persistStore(store);


// instead of defining the reducers in the reducer field of configureStore, combine them here:

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof rootReducer>;
// export type AppThunk<ReturnType = void> = ThunkAction<
//     ReturnType,
//     RootState,
//     unknown,
//     Action<string>
// >;

setupListeners(store.dispatch);

