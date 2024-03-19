import MenuItemSM from '@components/MenuItemSM';
import SunButton from '@components/buttons/SunButton';
import {
  FilesEditorIcon,
  HelpIcon,
  LogIcon,
  PlayListEditorIcon,
  QueueStatisIcon,
  SDChannelIcon,
  SDIcon,
  SettingsEditorIcon,
  SideBarMenuIcon,
  StreamGroupEditorIcon,
  StreamingStatusIcon,
  VideoPlayerIcon
} from '@lib/common/icons';
import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsGetAPI';

import useSettings from '@lib/useSettings';
import PrimeReact, { PrimeReactContext } from 'primereact/api';
import { useLocalStorage } from 'primereact/hooks';
import { Tooltip } from 'primereact/tooltip';
import { useCallback, useContext, useEffect, useState } from 'react';
import { Menu, MenuItem, Sidebar, sidebarClasses } from 'react-pro-sidebar';
export const RootSideBar = () => {
  const [dark, setDark] = useLocalStorage(true, 'dark');
  // const [currentDark, setCurrentDark] = useSessionStorage<boolean | null>(null, 'currentDark');
  const context = useContext(PrimeReactContext);

  const [collapsed, setCollapsed] = useLocalStorage<boolean>(true, 'app-menu-collapsed');
  const [isReady, setIsReady] = useState(false);
  const settings = useSettings();

  useEffect(() => {
    const intervalId = setInterval(() => {
      GetIsSystemReady()
        .then((result) => {
          setIsReady(result ?? false);
        })
        .catch(() => {
          setIsReady(false);
        });
    }, 5000);

    return () => clearInterval(intervalId);
  }, []);

  const onsetCollapsed = useCallback(
    (isCollapsed: boolean) => {
      setCollapsed(isCollapsed);
    },
    [setCollapsed]
  );

  const setTheme = useCallback(
    (intDark: boolean, callback?: () => void) => {
      const newTheme = intDark ? 'dark' : 'light';
      const theme = !intDark ? 'dark' : 'light';

      if (context?.changeTheme) {
        context.changeTheme(theme, newTheme, 'theme-link', callback);
      } else if (PrimeReact?.changeTheme) {
        PrimeReact.changeTheme(theme, newTheme, 'theme-link', callback);
      }
    },
    [context]
  );

  const toggleTheme = () => {
    setTheme(!dark, () => {
      setDark(!dark);
    });
  };

  // useEffect(() => {
  //   if (currentDark === null || currentDark !== dark) {
  //     setTheme(dark, () => {
  //       console.log('setTheme', dark);
  //       setCurrentDark(dark);
  //     });
  //   }
  // }, [currentDark, dark, setCurrentDark, setTheme]);

  return (
    <Sidebar
      className="app sidebar max-h-screen justify-content-start align-items-start"
      collapsed={collapsed}
      collapsedWidth="52px"
      rootStyles={{
        [`.${sidebarClasses.container}`]: {
          backgroundColor: 'var(--surface-a)'
        }
      }}
      style={{ height: 'calc(100vh - 10px)' }}
    >
      <Menu
        menuItemStyles={{
          button: ({ active }) => ({
            '&:hover': {
              backgroundColor: '#cb5e00'
            },
            backgroundColor: active ? '#cb5e00' : undefined
          })
        }}
      >
        <div
          onClick={() => {
            onsetCollapsed(!collapsed);
          }}
        >
          <MenuItem icon={<SideBarMenuIcon sx={{ color: 'var(--orange-color)', fontSize: 32 }} />}>
            <h2 className="orange-color">Stream Master</h2>
          </MenuItem>
        </div>
        {/* <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/testpanel" name='Test Panel' /> */}
        <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/editor/streams" name="Streams" />
        <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/editor/playlist" name="Playlist" />
        <MenuItemSM collapsed={collapsed} icon={<StreamGroupEditorIcon />} link="/editor/streamgroup" name="Stream Group" />
        <MenuItemSM collapsed={collapsed} icon={<FilesEditorIcon />} link="/editor/files" name="Files" />
        {settings.data.hls?.hlsM3U8Enable === true ? <MenuItemSM collapsed={collapsed} icon={<VideoPlayerIcon />} link="/viewer/player" name="Player" /> : null}
        {settings.data.sdSettings?.sdEnabled === true ? (
          <MenuItemSM collapsed={collapsed} icon={<SDIcon />} link="/editor/sdHeadEndLineUps" name="SD HeadEnds" />
        ) : null}
        {settings.data.sdSettings?.sdEnabled === true ? (
          <MenuItemSM collapsed={collapsed} icon={<SDChannelIcon />} link="/editor/sdChannels" name="SD Channels" />
        ) : null}
        <MenuItemSM collapsed={collapsed} icon={<StreamingStatusIcon />} link="/streamingstatus" name="Status" />
        <MenuItemSM collapsed={collapsed} icon={<QueueStatisIcon />} link="/queuestatus" name="Queue" />
        <MenuItemSM collapsed={collapsed} icon={<SettingsEditorIcon />} link="/settings" name="Settings" />
        <MenuItemSM collapsed={collapsed} icon={<LogIcon />} link="/viewer/logviewer" name="Log" />
        <MenuItemSM collapsed={collapsed} icon={<HelpIcon />} link="https://github.com/SenexCrenshaw/StreamMaster/wiki" name="Wiki" newWindow />

        <MenuItem
          component={
            <SunButton
              isDark={dark}
              onClick={(e) => {
                toggleTheme();
              }}
            />
          }
        />
      </Menu>

      <div className="absolute bottom-0 left-0 pb-2 flex flex-column m-0 p-0">
        <div className="col-6 p-0 m-0 justify-content-center align-content-center">
          <img className="p-0 m-0" alt="Stream Master Logo" src={isReady ? '/images/StreamMasterx32Ready.png' : '/images/StreamMasterx32NotReady.png'} />
        </div>

        <Tooltip target=".custom-target-icon" />
        <div
          className="custom-target-icon col-6 m-0 p-0 justify-content-center align-content-start text-xs text-center"
          data-pr-position="right"
          data-pr-tooltip={settings.data.release ?? ''}
        >
          {settings.data.version ?? ''}
        </div>
      </div>
    </Sidebar>
  );
};
