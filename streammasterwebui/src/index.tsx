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

import { ProSidebarProvider } from 'react-pro-sidebar';
import App from './App';
import { Navigate, Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import StreamGroupEditor from './features/streamGroupEditor/StreamGroupEditor';
import PlayListEditor from './features/playListEditor/PlayListEditor';
import QueueStatus from './features/queueStatus/QueueStatus';
import SettingsEditor from './features/settings/SettingsEditor';
import StreamingStatus from './features/streamingStatus/StreamingStatus';

const rootElement = document.getElementById("root");
if (!rootElement) throw new Error('Failed to find the root element');
const root = createRoot(rootElement);

PrimeReact.ripple = false;

const router = createBrowserRouter(
  createRoutesFromElements(
    <Route element={<App />} path="/">
      <Route element={<Navigate to="/editor/playlist" />} index />

      <Route element={<StreamGroupEditor />} path="/editor/streamgroup" />
      <Route element={<PlayListEditor />} path="/editor/playlist" />
      <Route element={<StreamingStatus />} index path="/streamingstatus" />
      <Route element={<QueueStatus />} path="/queuestatus" />

      <Route element={<SettingsEditor />} path="/settings" />

      <Route
        element={<Navigate replace to="/editor/streamgroup" />}
        path="*"
      />

    </Route>
  )
);

root.render(
  <React.StrictMode>
    <Provider store={store}>
      <ProSidebarProvider>
        <RouterProvider router={router} />
      </ProSidebarProvider>
    </Provider>
  </React.StrictMode>
);
