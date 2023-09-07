import './App.css';

import React from 'react';
import { Navigate, Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import messagesEn from './messages_en';
import * as StreamMasterApi from './store/iptvApi';

import { useLocalStorage } from 'primereact/hooks';
import { IntlProvider } from 'react-intl';
import { ProSidebarProvider } from 'react-pro-sidebar';

import Home from './Home';
import SignalRHub from './app/SignalRHub';
import VideoPlayer from './components/VideoPlayer';
import FilesEditor from './features/filesEditor/FilesEditor';
import LogViewer from './features/logViewer/LogViewer';
import PlayListEditor from './features/playListEditor/PlayListEditor';
import QueueStatus from './features/queueStatus/QueueStatus';
import SDEditor from './features/sdEditor/SDEditor';
import SettingsEditor from './features/settings/SettingsEditor';
import StreamGroupEditor from './features/streamGroupEditor/StreamGroupEditor';
import StreamingStatus from './features/streamingStatus/StreamingStatus';
// import TestPanel from './features/testPanel/TestPanel';
import TestPanel from './features/testPanel/TestPanel';
import StreamMasterSetting from './store/signlar/StreamMasterSetting';

// import SDEditor from './features/sdEditor/SDEditor';

const App = () => {
  const [locale,] = useLocalStorage('en', 'locale');
  const messages = locale === 'en' ? messagesEn : messagesEn;
  const [hubConnected, setHubConnected] = React.useState<boolean>(false);
  const systemStatus = StreamMasterApi.useSettingsGetSystemStatusQuery();
  const setting = StreamMasterSetting();

  const systemReady = React.useMemo((): boolean => {
    if (!hubConnected) {
      return false;
    }

    if (!systemStatus.data?.isSystemReady) {
      return false;
    }

    if (setting.isLoading || setting.defaultIcon === '') {
      return false;
    }

    return systemStatus.data.isSystemReady;

  }, [hubConnected, setting, systemStatus.data]);


  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route element={<Home />} path="/">
        <Route element={<Navigate to="/editor/playlist" />} index />

        <Route element={
          <TestPanel />
        } path="/testpanel" />

        <Route element={
          <StreamGroupEditor />
        } path="/editor/streamgroup" />

        <Route element={
          <PlayListEditor />
        } path="/editor/playlist" />

        <Route element={
          <FilesEditor />
        } path="/editor/files" />

        <Route element={
          <SDEditor />
        } path="/editor/sd" />

        <Route element={
          <StreamingStatus />
        } path="/streamingstatus" />

        <Route element={
          <QueueStatus />
        } path="/queuestatus" />

        <Route element={
          <VideoPlayer />
        } path="/player" />

        <Route element={
          <SettingsEditor />
        } path="/settings" />

        <Route element={
          <LogViewer />
        } path="/log" />


      </Route>

    )
  );

  if (!systemReady) {

    if (window.location.pathname === '/') {
      window.location.href = '/editor/playlist'

      return null;
    }

    return (
      <div className="flex justify-content-center flex-wrap card-container w-full h-full "  >

        <SignalRHub onConnected={(e) => setHubConnected(e)} />

        <div className="flex align-items-center justify-content-center font-bold  m-2"
          style={{
            height: 'calc(100vh - 10px)',
          }}
        >
          <div className='flex align-items-center justify-content-center'>
            <h3>Loading...</h3>
            <img
              alt='Logo'
              className="App-logo max-h-full max-w-full p-0"
              src='/images/sm.gif'
            />
          </div>
        </div>
      </div >
    );
  }

  return (
    <IntlProvider locale={locale} messages={messages}>
      <SignalRHub />
      <ProSidebarProvider>
        <RouterProvider router={router} />
      </ProSidebarProvider>
    </IntlProvider>
  );
};

export default React.memo(App);
