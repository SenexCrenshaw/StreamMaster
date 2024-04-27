import { configureStore, type Action, type ThunkAction } from '@reduxjs/toolkit';
import { persistStore } from 'redux-persist';
import { rootReducer } from './reducers';

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

// const rootReducer = combineReducers({
//   appInfo: appInfoSliceReducer,
//   any: anySlice,
//   channelGroupToRemove: channelGroupToRemoveSliceReducer,
//   queryAdditionalFilters: queryAdditionalFiltersReducer,
//   queryFilter: queryFilterReducer,
//   GetPagedStreamGroups: GetPagedStreamGroups,
//   GetPagedChannelGroups: GetPagedChannelGroups,
//   GetStreamGroupSMChannels: StreamGroupSMChannelLinks,
//   GetPagedM3UFiles: GetPagedM3UFiles,
//   GetPagedSMChannels: GetPagedSMChannels,
//   GetPagedSMStreams: GetPagedSMStreams,
//   GetSettings: GetSettings,
//   GetIsSystemReady: GetIsSystemReady,
//   GetSystemStatus: GetSystemStatus,
//   GetIcons: GetIcons,
//   GetSMChannelStreams: GetSMChannelStreams,
//   SMChannelReducer: SMChannelReducer,
//   SMStreamReducer: SMStreamReducer,
//   GetEPGColors: GetEPGColorsSlice,
//   GetEPGFiles: GetEPGFiles,
//   GetStreamGroups: GetStreamGroups,
//   GetPagedEPGFiles: GetPagedEPGFiles,
//   GetEPGFilePreviewById: GetEPGFilePreviewById,
//   GetEPGNextEPGNumber: GetEPGNextEPGNumber,
//   GetStationChannelNames: GetStationChannelNames,
//   selectUpdateSettingRequest: persistReducer(selectUpdateSettingRequestSliceConfig, selectUpdateSettingRequestReducer),
//   selectCurrentSettingDto: persistReducer(currentSettingDtoSliceConfig, selectCurrentSettingDtoReducer),
//   selectedPostalCode: persistReducer(selectedPostalCodeConfig, selectedPostalCodeSlice),
//   selectAll: persistReducer(selectAllConfig, selectAllSliceReducer),
//   selectedCountry: persistReducer(selectedCountryConfig, selectedCountrySlice),
//   selectedItems: persistReducer(selectedItemsConfig, selectedItemsSliceReducer),
//   selectedStreamGroup: persistReducer(selectedStreamGroupConfig, selectedStreamGroupSliceReducer),
//   selectedVideoStreams: persistReducer(selectedVideoStreamsConfig, selectedVideoStreamsSliceReducer),
//   showHidden: persistReducer(showHiddenConfig, showHiddenSliceReducer),
//   showSelections: persistReducer(showSelectionsConfig, showSelectionsSliceReducer),
//   sortInfo: persistReducer(sortInfoConfig, sortInfoSliceReducer),
//   selectSMStreams: persistReducer(selectSMStreamsConfig, selectSMStreamsReducer),
//   messages: SMMessagesReducer
