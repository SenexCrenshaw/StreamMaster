/* eslint-disable @typescript-eslint/no-unused-vars */
import './App.css';

import { Outlet } from 'react-router-dom';

import * as StreamMasterApi from './store/iptvApi';
import { BlockUI } from 'primereact/blockui';
import React from 'react';
import messagesEn from './messages_en';
import { hubConnection } from './app/store';
import { HubConnectionState } from '@microsoft/signalr';

import { Sidebar, Menu, MenuItem, sidebarClasses } from 'react-pro-sidebar';

import { useSessionStorage } from 'primereact/hooks';
import MenuItemSM from './components/MenuItemSM';
import { HelpIcon, PlayListEditorIcon, QueueStatisIcon, SettingsEditorIcon, SideBarMenuIcon, StreamGroupEditorIcon, StreamingStatusIcon, VideoIcon } from './common/icons';
import StreamMasterSetting from './store/signlar/StreamMasterSetting';
import { IntlProvider } from 'react-intl';
import { isDev } from './settings';

const App = () => {
  const settings = StreamMasterSetting();
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const [locale, setLocale] = React.useState('en');
  const messages = locale === 'en' ? messagesEn : messagesEn;

  const [collapsed, setCollapsed] = useSessionStorage<boolean>(true, 'app-menu-collapsed');
  const [hubConnected, setHubConnected] = React.useState<boolean>(false);
  // const [previousLocation, setPreviousLocation] = React.useState<string>('');

  // const location = useLocation();

  const onsetCollapsed = React.useCallback((isCollapsed: boolean) => {
    setCollapsed(isCollapsed);
  }, [setCollapsed]);

  React.useEffect(() => {

    const interval = setInterval(() => {
      if (hubConnection.state === HubConnectionState.Connected && !hubConnected) {
        setHubConnected(true);
        console.log("App Connected");

      } else if (hubConnection.state !== HubConnectionState.Connected && hubConnected) {
        setHubConnected(false);
        console.log("App Disconnected");
      }

    }, 1000);

    return () => clearInterval(interval); // This represents the unmount function, in which you need to clear your interval to prevent memory leaks.
  }, [hubConnected])

  // React.useEffect(() => {

  //   if (location.pathname !== previousLocation) {
  //     setPreviousLocation(location.pathname);
  //     if (!collapsed) {
  //       setTimeout(() => {
  //         setCollapsed(true);
  //       }, 1000);

  //     }
  //   }

  // }, [collapsed, location, previousLocation, setCollapsed]);

  const systemStatus = StreamMasterApi.useSettingsGetSystemStatusQuery();

  const systemReady = React.useMemo((): boolean => {
    if (!hubConnected) {
      return false;
    }

    if (!systemStatus.data?.isSystemReady) {
      return false;
    }

    return systemStatus.data.isSystemReady;

  }, [hubConnected, systemStatus.data]);


  StreamMasterApi.useChannelGroupsGetChannelGroupsQuery();
  StreamMasterApi.useEpgFilesGetEpgFilesQuery();
  StreamMasterApi.useM3UFilesGetM3UFilesQuery();
  StreamMasterApi.useProgrammesGetProgrammeNamesQuery();
  StreamMasterApi.useSettingsGetSettingQuery();
  StreamMasterApi.useSettingsGetSystemStatusQuery();
  StreamMasterApi.useStreamGroupsGetStreamGroupsQuery();
  StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();

  if (!systemReady)
    return (
      <div className="flex justify-content-center flex-wrap card-container w-full h-full "  >
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
      <div className='flex max-h-screen'>
        <Sidebar
          className="app sidebar max-h-screen "
          defaultCollapsed={collapsed}
          rootStyles={{
            [`.${sidebarClasses.container}`]: {
              backgroundColor: 'var(--mask-bg)',
            },
          }}
          style={{ height: 'calc(100vh - 10px)', }}

        >
          <Menu
            menuItemStyles={{
              button: ({ active }) => {
                return {
                  '&:hover': {
                    backgroundColor: '#cb5e00',
                  },
                  backgroundColor: active ? '#cb5e00' : undefined,
                };
              }
            }}
          >
            <div onClick={() => { onsetCollapsed(!collapsed); }}>
              <MenuItem
                className="menu1"
                icon={<SideBarMenuIcon sx={{ color: '#FE7600', fontSize: 32 }} />}
              >
                <h2
                  style={{
                    color: '#FE7600',
                  }}
                >Stream Master</h2>
              </MenuItem>
            </div>


            <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/editor/playlist" name='Playlist' />
            <MenuItemSM collapsed={collapsed} icon={<StreamGroupEditorIcon />} link="/editor/streamgroup" name='Stream Group' />

            <MenuItemSM collapsed={collapsed} icon={<StreamingStatusIcon />} link="/streamingstatus" name='Status' />
            <MenuItemSM collapsed={collapsed} icon={<QueueStatisIcon />} link="/queuestatus" name='Queue' />
            <MenuItemSM collapsed={collapsed} icon={<SettingsEditorIcon />} link="/settings" name='Settings' />

            {/* {isDev &&
              <MenuItemSM collapsed={collapsed} icon={<VideoIcon />} link="/player" name='Video Player' newWindow />
            } */}

            <MenuItemSM collapsed={collapsed} icon={<HelpIcon />} link="https://github.com/SenexCrenshaw/StreamMaster/wiki" name='Wiki' newWindow />

          </Menu>

          <div className='absolute bottom-0 left-0 pb-2 flex flex-column m-0 p-0 justify-content-center align-items-center'>
            <div className='flex col-12 justify-content-center align-items-center'>
              <img
                alt='Stream Master Logo'
                height={32}
                src="/images/StreamMasterx32.png"
                style={{
                  objectFit: 'contain',
                }}
              />
            </div>
            <div className='flex flex-column m-0 p-0 justify-content-center align-items-center text-xs text-center'>
              {settings.data.version ?? ''}
            </div>
          </div>
        </Sidebar>


        <div className="flex ml-2 flex-grow-1 max-h-screen max-w-full justify-content-between align-items-start">
          <BlockUI blocked={!systemReady} className="flex flex-grow-1 max-h-screen max-w-full justify-content-between">
            <Outlet />
          </BlockUI >
        </div>
      </div >
    </IntlProvider>
  );
};

export default React.memo(App);
