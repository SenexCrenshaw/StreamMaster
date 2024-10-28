import { Navigate, Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';

import { useStore } from 'react-redux';
import { persistStore } from 'redux-persist';
import { RootLayout } from './RootLayout';

import '@lib/styles/index.css';

import 'primeicons/primeicons.css'; //icons
import 'primereact/resources/primereact.min.css'; //core css
// import 'primereact/resources/themes/viva-dark/theme.css'; // theme
import SMLoader from '@components/loader/SMLoader';

import { SettingsProvider } from '@lib/context/SettingsProvider';
import { Suspense, lazy } from 'react';
import RequireAuth from './RequireAuth';

const Router = (): JSX.Element => {
  const store = useStore();

  const StreamEditor = lazy(() => import('@features/streameditor/StreamEditor'));
  const SettingsEditor = lazy(() => import('@features/settings/SettingsEditor'));
  const SDEditor = lazy(() => import('@features/sdEditor/SDEditor'));
  // const TestPanel = lazy(() => import('./testing/TestPanel'));
  // const FilesEditor = lazy(() => import('@features/filesEditor/FilesEditor'));
  // const LogViewer = lazy(() => import('@features/logViewer/LogViewer'));
  // const PlayListEditor = lazy(() => import('@features/playListEditor/PlayListEditor'));
  // const QueueStatus = lazy(() => import('@features/queueStatus/QueueStatus'));
  // const SDEditorChannels = lazy(() => import('@features/sdEditor/SDEditorChannels'));
  // const SDEditorHeadEndsAndLineUps = lazy(() => import('@features/sdEditor/SDEditorHeadEndsAndLineUps'));
  // const SettingsEditor = lazy(() => import('@features/settings/SettingsEditor'));
  // const StreamGroupEditor = lazy(() => import('@features/streamGroupEditor/StreamGroupEditor'));
  const StreamingStatus = lazy(() => import('@features/streamingStatus/StreamingStatus'));
  // const VideoPlayer = lazy(() => import('@features/videoPlayer/VideoPlayer'));

  const persistor = persistStore(store, {}, () => {
    persistor.persist();
  });

  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route
        element={
          <RequireAuth>
            <RootLayout />
          </RequireAuth>
        }
        path="/"
      >
        <Route
          element={
            <RequireAuth>
              <Suspense>
                <Navigate to="/editor/streams" />
              </Suspense>
            </RequireAuth>
          }
          index
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
              <SettingsProvider>
                <SettingsEditor />
              </SettingsProvider>
            </Suspense>
          }
          path="/settings"
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
              <SMLoader />
            </Suspense>
          }
          path="/loader"
        />
        <Route
          element={
            <Suspense>
              <SDEditor />
            </Suspense>
          }
          path="/editor/sdHeadEndLineUps"
        />

        {/* <Route
          element={
            <Suspense>
              <SDEditorChannels />
            </Suspense>
          }
          path="/editor/sdChannels"
        /> */}

        {/* <Route
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
        /> */}
      </Route>
    )
  );

  // useSchedulesDirectGetStationChannelNamesQuery();
  // useEpgFilesGetEpgColorsQuery();
  // useIconsGetIconsQuery();

  return <RouterProvider router={router} />;
};

export default Router;
