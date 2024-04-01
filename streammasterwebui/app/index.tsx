import store, { persistor } from '@lib/redux/store';
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
      <PrimeReactProvider value={{ ripple: false, inputStyle: 'outlined' }}>
        <App />
      </PrimeReactProvider>
    </Provider>
  </React.StrictMode>
);
