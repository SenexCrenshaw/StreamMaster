
// import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query/react';
import { combineReducers } from 'redux';

import { enhancedApi } from '../store/signlar/enhancedApi';
import { enhancedApiLocal } from '../store/signlar/enhancedApiLocal';

const rootReducer = combineReducers({
  [enhancedApi.reducerPath]: enhancedApi.reducer,
});

export const store = configureStore({
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      immutableCheck: false,
      serializableCheck: false,
    }).concat(enhancedApi.middleware, enhancedApiLocal.middleware),
  reducer: rootReducer,
});

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

