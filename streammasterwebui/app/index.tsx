import store, { persistor } from '@lib/redux/store';
import { SMProvider } from '@lib/signalr/SMProvider';
import { PrimeReactProvider } from 'primereact/api';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import App from './App';

const root = ReactDOM.createRoot(document.querySelector('#root') as HTMLElement);

root.render(
  <React.StrictMode>
    <Provider store={store}>
      <PersistGate persistor={persistor} />
      <PrimeReactProvider value={{ inputStyle: 'outlined', ripple: false }}>
        <SMProvider>
          <App />
        </SMProvider>
      </PrimeReactProvider>
    </Provider>
  </React.StrictMode>
);
