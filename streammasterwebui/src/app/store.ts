import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query/react';
import { combineReducers } from 'redux';

import { enhancedApi } from '../store/signlar/enhancedApi';
import { enhancedApiLocal } from '../store/signlar/enhancedApiLocal';
import queryFilterReducer from './slices/queryFilterSlice';
import queryAdditionalFiltersReducer from './slices/queryAdditionalFiltersSlice';
import streamToRemoveSliceReducer from './slices/streamToRemoveSlice';
import streamGroupToRemoveSliceReducer from './slices/streamGroupToRemoveSlice';
import channelGroupToRemoveSliceReducer from './slices/channelGroupToRemoveSlice';
import selectAllSliceReducer from './slices/selectAllSlice';
import sortInfoSliceReducer from './slices/sortInfoSlice';
import showHiddenSliceReducer from './slices/showHiddenSlice';
import { persistReducer, persistStore } from 'redux-persist';
import storage from 'redux-persist/lib/storage';

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

const rootReducer = combineReducers({
  [enhancedApi.reducerPath]: enhancedApi.reducer,
  channelGroupToRemove:channelGroupToRemoveSliceReducer,
  queryAdditionalFilters: queryAdditionalFiltersReducer,
  queryFilter: queryFilterReducer,
  selectAll:persistReducer(selectAllConfig, selectAllSliceReducer),
  showHidden: persistReducer(showHiddenConfig, showHiddenSliceReducer),
  sortInfo: persistReducer(sortInfoConfig, sortInfoSliceReducer),
  streamGroupToRemove:streamGroupToRemoveSliceReducer,
  streamToRemove: streamToRemoveSliceReducer,
});


export const store = configureStore({
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      immutableCheck: false,
      serializableCheck: false,
    }).concat(enhancedApi.middleware, enhancedApiLocal.middleware),
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

