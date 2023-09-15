import { useLocalStorage } from 'primereact/hooks';
import { Tooltip } from 'primereact/tooltip';
import React from 'react';
import { Menu, MenuItem, Sidebar, sidebarClasses } from 'react-pro-sidebar';
import { Outlet } from 'react-router-dom';
import './App.css';
import { FilesEditorIcon, HelpIcon, LogIcon, PlayListEditorIcon, QueueStatisIcon, SettingsEditorIcon, SideBarMenuIcon, StreamGroupEditorIcon, StreamingStatusIcon } from './common/icons';
import MenuItemSM from './components/MenuItemSM';
import { useSettingsGetSettingQuery } from './store/iptvApi';
import StreamMasterSetting from './store/signlar/StreamMasterSetting';

const Home = () => {
  useSettingsGetSettingQuery();

  const settings = StreamMasterSetting();
  const [collapsed, setCollapsed] = useLocalStorage<boolean>(true, 'app-menu-collapsed');

  const onsetCollapsed = React.useCallback((isCollapsed: boolean) => {
    setCollapsed(isCollapsed);
  }, [setCollapsed]);



  return (
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
              icon={<SideBarMenuIcon sx={{ color: 'var(--orange-color)', fontSize: 32 }} />}
            >
              <h2
                style={{
                  color: 'var(--orange-color)',
                }}
              >Stream Master</h2>
            </MenuItem>
          </div>

          {/* <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/testpanel" name='Test Panel' /> */}

          <MenuItemSM collapsed={collapsed} icon={<PlayListEditorIcon />} link="/editor/playlist" name='Playlist' />
          <MenuItemSM collapsed={collapsed} icon={<StreamGroupEditorIcon />} link="/editor/streamgroup" name='Stream Group' />

          <MenuItemSM collapsed={collapsed} icon={<FilesEditorIcon />} link="/editor/files" name='Files' />

          <MenuItemSM collapsed={collapsed} icon={<StreamingStatusIcon />} link="/streamingstatus" name='Status' />
          <MenuItemSM collapsed={collapsed} icon={<QueueStatisIcon />} link="/queuestatus" name='Queue' />
          <MenuItemSM collapsed={collapsed} icon={<SettingsEditorIcon />} link="/settings" name='Settings' />

          {/* <MenuItemSM collapsed={collapsed} icon={<SDIcon />} link="/editor/sd" name='Schedules Direct' /> */}
          {/* <MenuItemSM collapsed={collapsed} icon={<VideoIcon />} link="/player" name='Video Player' /> */}

          <MenuItemSM collapsed={collapsed} icon={<LogIcon />} link="/log" name='Log' />
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
          <Tooltip target=".custom-target-icon" />
          <div
            className='custom-target-icon flex flex-column m-0 p-0 justify-content-center align-items-center text-xs text-center'
            data-pr-position="right"
            data-pr-tooltip={settings.data.release ?? ''}
          >
            {settings.data.version ?? ''}
          </div>
        </div>
      </Sidebar>

      <div className='flex flex-column w-full'>
        <Outlet />
      </div>
    </div >

  );
};

export default React.memo(Home);
