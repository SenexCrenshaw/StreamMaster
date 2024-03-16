import { useLocalStorage } from 'primereact/hooks';
import { Navigate, Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';

import FilesEditor from '@features/filesEditor/FilesEditor';
import LogViewer from '@features/logViewer/LogViewer';
import PlayListEditor from '@features/playListEditor/PlayListEditor';
import QueueStatus from '@features/queueStatus/QueueStatus';
import SDEditorChannels from '@features/sdEditor/SDEditorChannels';
import SDEditorHeadEndsAndLineUps from '@features/sdEditor/SDEditorHeadEndsAndLineUps';
import SettingsEditor from '@features/settings/SettingsEditor';
import StreamGroupEditor from '@features/streamGroupEditor/StreamGroupEditor';
import StreamEditor from '@features/streameditor/StreamEditor';
import StreamingStatus from '@features/streamingStatus/StreamingStatus';
import VideoPlayer from '@features/videoPlayer/VideoPlayer';
import {
  IconFileDto,
  StationChannelName,
  useEpgFilesGetEpgColorsQuery,
  useIconsGetIconsQuery,
  useSchedulesDirectGetStationChannelNamesQuery
} from '@lib/iptvApi';
import MessagesEn from '@lib/locales/MessagesEn';
import { CacheProvider } from '@lib/redux/CacheProvider';
import { SignalRConnection } from '@lib/signalr/SignalRConnection';

import { IntlProvider } from 'react-intl';
import { useStore } from 'react-redux';
import { persistStore } from 'redux-persist';
import { RootLayout } from './RootLayout';
import TestPanel from './testing/TestPanel';

import '@lib/styles/index.css';
// import '@lib/styles/streammaster-dark.css'; //theme
// import '@lib/styles/theme.css'; // theme
import 'primeicons/primeicons.css'; //icons
import 'primereact/resources/primereact.min.css'; //core css

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

        <Route element={<TestPanel />} path="/testpanel" />

        <Route element={<StreamGroupEditor />} path="/editor/streamgroup" />

        <Route element={<PlayListEditor />} path="/editor/playlist" />

        <Route element={<FilesEditor />} path="/editor/files" />

        <Route element={<SDEditorHeadEndsAndLineUps />} path="/editor/sdHeadEndLineUps" />

        <Route element={<SDEditorChannels />} path="/editor/sdChannels" />

        <Route element={<StreamEditor />} path="/editor/streams" />

        <Route element={<StreamingStatus />} path="/streamingstatus" />

        <Route element={<QueueStatus />} path="/queuestatus" />

        <Route element={<SettingsEditor />} path="/settings" />

        <Route element={<LogViewer />} path="/viewer/logviewer" />
        <Route element={<VideoPlayer />} path="/viewer/player" />
      </Route>
    )
  );
  useSchedulesDirectGetStationChannelNamesQuery();
  useEpgFilesGetEpgColorsQuery();
  useIconsGetIconsQuery();

  return (
    <div className="App p-fluid">
      <IntlProvider locale={locale} messages={messages}>
        <SignalRConnection>
          <CacheProvider<IconFileDto>>
            <CacheProvider<StationChannelName>>
              <RouterProvider router={router} />
            </CacheProvider>
          </CacheProvider>
        </SignalRConnection>
      </IntlProvider>
    </div>
  );
};

export default App;
