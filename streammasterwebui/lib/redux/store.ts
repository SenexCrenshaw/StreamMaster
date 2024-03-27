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
import SMChannelsSlice from '@lib/smAPI/SMChannels/SMChannelsSlice';
import SMStreamsReducer from '@lib/smAPI/SMStreams/SMStreamsSlice';
import M3UFilesReducer from '@lib/smAPI/M3UFiles/M3UFilesSlice';
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

  channelGroupToRemove: channelGroupToRemoveSliceReducer,
  queryAdditionalFilters: queryAdditionalFiltersReducer,
  queryFilter: queryFilterReducer,
  SMStreams: SMStreamsReducer,
  SMChannels: SMChannelsSlice,
  SMChannelReducer: SMChannelReducer,
  SMStreamReducer: SMStreamReducer,
  M3UFiles: M3UFilesReducer,
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
