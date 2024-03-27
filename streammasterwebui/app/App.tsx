import { useLocalStorage } from 'primereact/hooks';
import { Navigate, Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';

// import FilesEditor from '@features/filesEditor/FilesEditor';
// import LogViewer from '@features/logViewer/LogViewer';
// import PlayListEditor from '@features/playListEditor/PlayListEditor';
// import QueueStatus from '@features/queueStatus/QueueStatus';
// import SDEditorChannels from '@features/sdEditor/SDEditorChannels';
// import SDEditorHeadEndsAndLineUps from '@features/sdEditor/SDEditorHeadEndsAndLineUps';
// import SettingsEditor from '@features/settings/SettingsEditor';
// import StreamGroupEditor from '@features/streamGroupEditor/StreamGroupEditor';
// import StreamingStatus from '@features/streamingStatus/StreamingStatus';
// import VideoPlayer from '@features/videoPlayer/VideoPlayer';

import { useEpgFilesGetEpgColorsQuery, useIconsGetIconsQuery, useSchedulesDirectGetStationChannelNamesQuery } from '@lib/iptvApi';
import MessagesEn from '@lib/locales/MessagesEn';
// import { SignalRConnection } from '@lib/signalr/SignalRConnection';

import { IntlProvider } from 'react-intl';
import { useStore } from 'react-redux';
import { persistStore } from 'redux-persist';
import { RootLayout } from './RootLayout';
// import TestPanel from './testing/TestPanel';

import '@lib/styles/index.css';
// import '@lib/styles/streammaster-dark.css'; //theme
// import '@lib/styles/theme.css'; // theme
import 'primeicons/primeicons.css'; //icons
import 'primereact/resources/primereact.min.css'; //core css
import { MessageProcessor } from '@lib/signalr/MessageProcessor';
import { Suspense, lazy, useEffect } from 'react';
import SignalRService from '@lib/signalr/SignalRService';
import { SignalRProvider } from '@lib/signalr/SignalRProvider';
// import 'primereact/resources/primereact.css'; // core css
//import 'primereact/resources/themes/viva-dark/theme.css'; // theme

const App = (): JSX.Element => {
  const [locale] = useLocalStorage('en', 'locale');
  const messages = locale === 'en' ? MessagesEn : MessagesEn;
  const store = useStore();
  const TestPanel = lazy(() => import('./testing/TestPanel'));
  const StreamEditor = lazy(() => import('@features/streameditor/StreamEditor'));
  const FilesEditor = lazy(() => import('@features/filesEditor/FilesEditor'));
  const LogViewer = lazy(() => import('@features/logViewer/LogViewer'));
  const PlayListEditor = lazy(() => import('@features/playListEditor/PlayListEditor'));
  const QueueStatus = lazy(() => import('@features/queueStatus/QueueStatus'));
  const SDEditorChannels = lazy(() => import('@features/sdEditor/SDEditorChannels'));
  const SDEditorHeadEndsAndLineUps = lazy(() => import('@features/sdEditor/SDEditorHeadEndsAndLineUps'));
  const SettingsEditor = lazy(() => import('@features/settings/SettingsEditor'));
  const StreamGroupEditor = lazy(() => import('@features/streamGroupEditor/StreamGroupEditor'));
  const StreamingStatus = lazy(() => import('@features/streamingStatus/StreamingStatus'));
  const VideoPlayer = lazy(() => import('@features/videoPlayer/VideoPlayer'));
  const signalRService = SignalRService.getInstance();

  const persistor = persistStore(store, {}, () => {
    persistor.persist();
  });

  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route element={<RootLayout />} path="/">
        <Route
          element={
            <Suspense>
              <Navigate to="/editor/playlist" />
            </Suspense>
          }
          index
        />

        <Route
          element={
            <Suspense>
              <TestPanel />
            </Suspense>
          }
          path="/testpanel"
        />

        <Route
          element={
            <Suspense>
              <StreamGroupEditor />{' '}
            </Suspense>
          }
          path="/editor/streamgroup"
        />

        <Route
          element={
            <Suspense>
              <PlayListEditor />
            </Suspense>
          }
          path="/editor/playlist"
        />

        <Route
          element={
            <Suspense>
              <FilesEditor />
            </Suspense>
          }
          path="/editor/files"
        />

        <Route
          element={
            <Suspense>
              <SDEditorHeadEndsAndLineUps />
            </Suspense>
          }
          path="/editor/sdHeadEndLineUps"
        />

        <Route
          element={
            <Suspense>
              <SDEditorChannels />
            </Suspense>
          }
          path="/editor/sdChannels"
        />

        <Route
          element={
            <Suspense>
              <StreamEditor />
            </Suspense>
          }
          path="/editor/streams"
        />

        <Route
          element={
            <Suspense>
              <StreamingStatus />
            </Suspense>
          }
          path="/streamingstatus"
        />

        <Route
          element={
            <Suspense>
              <QueueStatus />
            </Suspense>
          }
          path="/queuestatus"
        />

        <Route
          element={
            <Suspense>
              <SettingsEditor />
            </Suspense>
          }
          path="/settings"
        />

        <Route
          element={
            <Suspense>
              <LogViewer />
            </Suspense>
          }
          path="/viewer/logviewer"
        />
        <Route
          element={
            <Suspense>
              <VideoPlayer />
            </Suspense>
          }
          path="/viewer/player"
        />
      </Route>
    )
  );

  useSchedulesDirectGetStationChannelNamesQuery();
  useEpgFilesGetEpgColorsQuery();
  useIconsGetIconsQuery();

  useEffect(() => {
    if (signalRService && signalRService.events) {
      signalRService.events((message) => {
        console.log(message);
      });
    }
  }, [signalRService]);

  return (
    <div className="App p-fluid">
      <IntlProvider locale={locale} messages={messages}>
        <MessageProcessor>
          <SignalRProvider>
            <RouterProvider router={router} />
          </SignalRProvider>
        </MessageProcessor>
      </IntlProvider>
    </div>
  );
};

export default App;
