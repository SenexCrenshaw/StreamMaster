import './App.css';

import { Navigate, Route, RouterProvider, createBrowserRouter, createRoutesFromElements } from 'react-router-dom';
import * as StreamMasterApi from './store/iptvApi';
import React, { version } from 'react';
import messagesEn from './messages_en';

import { ProSidebarProvider } from 'react-pro-sidebar';
import { useLocalStorage } from 'primereact/hooks';
import { IntlProvider } from 'react-intl';

import VideoPlayer from './components/VideoPlayer';
import PlayListEditor from './features/playListEditor/PlayListEditor';
import QueueStatus from './features/queueStatus/QueueStatus';
import SettingsEditor from './features/settings/SettingsEditor';
import StreamGroupEditor from './features/streamGroupEditor/StreamGroupEditor';
import StreamingStatus from './features/streamingStatus/StreamingStatus';
import ProtectedRoute from './_auth/ProtectedRoute';
import Login from './_auth/Login';
import { type UserInformation } from './common/common';
import Home from './Home';
import { apiKey, apiRoot, baseHostURL, isDev, requiresAuth, urlBase } from './settings';
import SignalRHub from './app/SignalRHub';

const App = () => {

  const [userInformation, setUserInformation] = useLocalStorage<UserInformation>({} as UserInformation, 'userInformation');
  const [locale,] = useLocalStorage('en', 'locale');
  const messages = locale === 'en' ? messagesEn : messagesEn;
  const [hubConnected, setHubConnected] = React.useState<boolean>(false);

  const setSignIn = React.useCallback((e: boolean) => {
    setUserInformation(
      {
        IsAuthenticated: e,
        TokenAge: new Date(),
      }
    )
  }, [setUserInformation]);

  React.useEffect(() => {
    if (requiresAuth !== true && userInformation.IsAuthenticated !== true) {
      setSignIn(true)
    }
  }, [setSignIn, userInformation.IsAuthenticated]);

  const systemStatus = StreamMasterApi.useSettingsGetSystemStatusQuery();

  const logOut = React.useCallback(() => {
    setUserInformation(
      {
        IsAuthenticated: false,
        TokenAge: new Date(),
      }
    )
  }, [setUserInformation]);

  const systemReady = React.useMemo((): boolean => {
    if (!hubConnected) {
      return false;
    }

    if (!systemStatus.data?.isSystemReady) {
      return false;
    }

    return systemStatus.data.isSystemReady;

  }, [hubConnected, systemStatus.data]);


  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route element={<Home />} path="/">
        <Route element={<Navigate to="/editor/playlist" />} index />

        <Route element={
          <ProtectedRoute isAuthenticated={userInformation.IsAuthenticated}  >
            <StreamGroupEditor />
          </ProtectedRoute>
        } path="/editor/streamgroup" />

        <Route element={
          <ProtectedRoute isAuthenticated={userInformation.IsAuthenticated}  >
            <PlayListEditor />
          </ProtectedRoute>
        } path="/editor/playlist" />

        <Route element={
          <ProtectedRoute isAuthenticated={userInformation.IsAuthenticated}  >
            <StreamingStatus />
          </ProtectedRoute>
        } path="/streamingstatus" />

        <Route element={
          <ProtectedRoute isAuthenticated={userInformation.IsAuthenticated}  >
            <QueueStatus />
          </ProtectedRoute>
        } path="/queuestatus" />

        <Route element={
          <ProtectedRoute isAuthenticated={userInformation.IsAuthenticated}  >
            <VideoPlayer />
          </ProtectedRoute>
        } path="/player" />

        <Route element={
          <ProtectedRoute isAuthenticated={userInformation.IsAuthenticated}  >
            <SettingsEditor isAuthenticated={userInformation.IsAuthenticated} logOut={() => logOut()} />
          </ProtectedRoute>
        } path="/settings" />

        <Route element={<Login onClose={(e) => { setSignIn(e) }} />} path="/login" />
      </Route>

    )
  );

  console.log('baseHostURL: ', baseHostURL)
  console.log('apiRoot: ', apiRoot)
  console.log('apiKey: ', apiKey)
  console.log('isDev: ', isDev)
  console.log('requiresAuth: ', requiresAuth)
  console.log('urlBase: ', urlBase)
  console.log('version: ', version)

  if (!systemReady)
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

  return (
    <IntlProvider locale={locale} messages={messages}>

      <SignalRHub onConnected={(e) => console.log('SignalRHub: ', e)} />

      <ProSidebarProvider>
        <RouterProvider router={router} />
      </ProSidebarProvider>
    </IntlProvider>
  );
};

export default React.memo(App);
