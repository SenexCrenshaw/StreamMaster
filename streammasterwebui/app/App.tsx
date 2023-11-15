import { useLocalStorage } from 'primereact/hooks';
import { Navigate, Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';

import '@lib/styles/dataSelector.css';
import '@lib/styles/index.css';
import '@lib/styles/theme.css'; // theme

import FilesEditor from '@features/filesEditor/FilesEditor';
import LogViewer from '@features/logViewer/LogViewer';
import PlayListEditor from '@features/playListEditor/PlayListEditor';
import QueueStatus from '@features/queueStatus/QueueStatus';
import SDEditorHeadEndsAndLineUps from '@features/sdEditor/SDEditorHeadEndsAndLineUps';
import SettingsEditor from '@features/settings/SettingsEditor';
import StreamGroupEditor from '@features/streamGroupEditor/StreamGroupEditor';
import StreamingStatus from '@features/streamingStatus/StreamingStatus';
import MessagesEn from '@lib/locales/MessagesEn';
import { SignalRConnection } from '@lib/signalr/SignalRConnection';
import 'primeflex/primeflex.css'; // css utility
import 'primeicons/primeicons.css'; // icons
import 'primereact/resources/primereact.css'; // core css
import 'primereact/resources/themes/viva-dark/theme.css'; // theme
import { IntlProvider } from 'react-intl';
import { ProSidebarProvider } from 'react-pro-sidebar';
import { useStore } from 'react-redux';
import { persistStore } from 'redux-persist';
import { RootLayout } from './RootLayout';
import SDEditorChannels from '@features/sdEditor/SDEditorChannels';

// const albert_sans = Albert_Sans({
//   subsets: ['latin'],
//   display: 'swap',
// });

const App = (): JSX.Element => {
  const [locale] = useLocalStorage('en', 'locale');
  const messages = locale === 'en' ? MessagesEn : MessagesEn;
  const store = useStore();
  const persistor = persistStore(store, {}, () => {
    persistor.persist();
  });

  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route element={<RootLayout />} path="/">
        <Route element={<Navigate to="/editor/playlist" />} index />

        {/* <Route element={<TestPanel />} path="/testpanel" /> */}

        <Route element={<StreamGroupEditor />} path="/editor/streamgroup" />

        <Route element={<PlayListEditor />} path="/editor/playlist" />

        <Route element={<FilesEditor />} path="/editor/files" />

        <Route element={<SDEditorHeadEndsAndLineUps />} path="/editor/sdHeadEndLineUps" />

        <Route element={<SDEditorChannels />} path="/editor/sdChannels" />

        <Route element={<StreamingStatus />} path="/streamingstatus" />

        <Route element={<QueueStatus />} path="/queuestatus" />

        <Route element={<SettingsEditor />} path="/settings" />

        <Route element={<LogViewer />} path="/viewer/logviewer" />
      </Route>
    )
  );

  return (
    <IntlProvider locale={locale} messages={messages}>
      <SignalRConnection>
        <ProSidebarProvider>
          <RouterProvider router={router} />
        </ProSidebarProvider>
      </SignalRConnection>
    </IntlProvider>
  );
};

export default App;
