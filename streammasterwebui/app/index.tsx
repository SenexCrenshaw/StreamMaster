import React from 'react';

import '@lib/styles/dataSelector.css';
import '@lib/styles/index.css';
import '@lib/styles/theme.css'; // theme

import store, { persistor } from '@lib/redux/store';
import 'primeflex/primeflex.css'; // css utility
import 'primeicons/primeicons.css'; // icons
import 'primereact/resources/primereact.css'; // core css
import 'primereact/resources/themes/viva-dark/theme.css'; // theme
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import App from './App';

const root = ReactDOM.createRoot(document.querySelector('#root') as HTMLElement);

root.render(
  <React.StrictMode>
    <Provider store={store}>
      <PersistGate persistor={persistor} />
      <App />
    </Provider>
  </React.StrictMode>
);
