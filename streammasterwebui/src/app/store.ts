
import {
  HttpTransportType,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
// import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack';
import { configureStore } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query/react';
import { combineReducers } from 'redux';
import { sleep } from '../common/common';
import { baseHostURL, hubName } from '../settings';
import { enhancedApi } from '../store/signlar/enhancedApi';
import { enhancedApiLocal } from '../store/signlar/enhancedApiLocal';
import userSlice from '../store/userSlice';

const rootReducer = combineReducers({
  [enhancedApi.reducerPath]: enhancedApi.reducer,
  ['user']: userSlice,
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


export const hubConnection = new HubConnectionBuilder()
  .configureLogging(LogLevel.Error)
  // .withHubProtocol(new MessagePackHubProtocol())
  .withUrl(baseHostURL + hubName, {
    skipNegotiation: true,
    transport: HttpTransportType.WebSockets,
  })
  .withAutomaticReconnect({
    nextRetryDelayInMilliseconds: retryContext => {
      if (retryContext.elapsedMilliseconds < 60000) {
        // If we've been reconnecting for less than 60 seconds so far,

        return 1000;
      } else {
        // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
        return 1000;
      }
    },
  })
  .build();

hubConnection.onreconnected(async () => {
  return console.error('onreconnected');
});


const onStartHub = () => {
  hubConnection.start().catch(function (err) {
    sleep(1000);
    onStartHub();
    return console.error(err.toString());
  }).finally(() => {
    if (hubConnection.state === HubConnectionState.Connected) {
      console.log('Hub Connected');
    }
  });
};

hubConnection.onclose(async () => {
  sleep(1000);
  onStartHub();
},
);


if (hubConnection.state === HubConnectionState.Disconnected) {
  onStartHub();
}

