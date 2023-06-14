/* eslint-disable @typescript-eslint/no-unused-vars */
import React from 'react';

import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';

import { store } from './app/store';

import 'normalize.css';

import 'primeflex/primeflex.css'; // css utility
import 'primeicons/primeicons.css'; // icons
import 'primereact/resources/primereact.css'; // core css
import 'primereact/resources/themes/viva-dark/theme.css'; // theme
import './theme.css'; // theme

import PrimeReact from 'primereact/api';

import './index.css';

import App from './App';

const rootElement = document.getElementById("root");
if (!rootElement) throw new Error('Failed to find the root element');
const root = createRoot(rootElement);

PrimeReact.ripple = false;

root.render(
  <React.StrictMode>
    <Provider store={store}>

      <App />
    </Provider>

  </React.StrictMode >
);
