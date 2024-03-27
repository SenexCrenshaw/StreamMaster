import store, { persistor } from '@lib/redux/store';
import { PrimeReactProvider } from 'primereact/api';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import App from './App';
// import { SignalRConnection } from '@lib/signalr/SignalRConnection';

const root = ReactDOM.createRoot(document.querySelector('#root') as HTMLElement);

root.render(
  <React.StrictMode>
    <Provider store={store}>
      {/* <SignalRConnection> */}
      <PersistGate persistor={persistor} />
      <PrimeReactProvider value={{ ripple: true, inputStyle: 'outlined' }}>
        <App />
      </PrimeReactProvider>
      {/* </SignalRConnection> */}
    </Provider>
  </React.StrictMode>
);
