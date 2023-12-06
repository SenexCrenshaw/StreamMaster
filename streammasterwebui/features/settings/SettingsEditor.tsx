import StandardHeader from '@components/StandardHeader';
import { GetMessage } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';
import { AuthenticationType } from '@lib/common/streammaster_enums';
import { SettingDto, useSettingsGetSettingQuery } from '@lib/iptvApi';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsMutateAPI';
import HistoryIcon from '@mui/icons-material/History';
import SaveIcon from '@mui/icons-material/Save';
import { Dock } from 'primereact/dock';
import { type MenuItem } from 'primereact/menuitem';
import { ScrollPanel } from 'primereact/scrollpanel';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { AuthenticationSettings } from './AuthenticationSettings';
import { DevelopmentSettings } from './DevelopmentSettings';
import { FilesEPGM3USettings } from './FilesEPGM3USettings';
import { GeneralSettings } from './GeneralSettings';
import { SDSettings } from './SDSettings';
import { StreamingSettings } from './StreamingSettings';

export const SettingsEditor = () => {
  const [newData, setNewData] = useState<SettingDto>({} as SettingDto);
  const [originalData, setOriginalData] = useState<SettingDto>({} as SettingDto);

  const settingsQuery = useSettingsGetSettingQuery();

  useEffect(() => {
    if (settingsQuery.isLoading || !settingsQuery.data) return;

    setNewData({ ...settingsQuery.data });
    setOriginalData({ ...settingsQuery.data });
  }, [settingsQuery]);

  const adminUserNameError = useMemo((): string | undefined => {
    if (newData.authenticationMethod === AuthenticationType.Forms && newData.adminUserName === '') return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [newData.adminUserName, newData.authenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (newData.authenticationMethod === AuthenticationType.Forms && newData.adminPassword === '') return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [newData.adminPassword, newData.authenticationMethod]);

  const isSaveEnabled = useMemo((): boolean => {
    if (JSON.stringify(newData) === JSON.stringify(originalData)) return false;

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
      return false;
    }

    if (newData.enableSSL === true && newData.sslCertPath === '') {
      return false;
    }

    return true;
  }, [adminPasswordError, adminUserNameError, newData, originalData]);

  const onSave = useCallback(() => {
    if (!isSaveEnabled) {
      return;
    }

    UpdateSetting(newData)
      .then(() => {})
      .catch(() => {});
  }, [isSaveEnabled, newData]);

  const items: MenuItem[] = [
    {
      command: () => {
        onSave();
      },
      disabled: !isSaveEnabled,
      icon: <SaveIcon sx={{ fontSize: 40 }} />,
      label: 'Save'
    },
    {
      command: () => {
        setNewData({ ...originalData });
      },
      disabled: !isSaveEnabled,
      icon: <HistoryIcon sx={{ fontSize: 40 }} />,
      label: 'Undo'
    }
  ];

  return (
    <StandardHeader displayName={GetMessage('settings')} icon={<SettingsEditorIcon />}>
      <ScrollPanel style={{ height: 'calc(100vh - 58px)', width: '100%' }}>
        <Dock model={items} position="right" />

        <GeneralSettings newData={newData} setNewData={setNewData} />

        <AuthenticationSettings newData={newData} setNewData={setNewData} />

        <StreamingSettings newData={newData} setNewData={setNewData} />

        <SDSettings newData={newData} setNewData={setNewData} />

        <FilesEPGM3USettings newData={newData} setNewData={setNewData} />

        <DevelopmentSettings newData={newData} setNewData={setNewData} />
      </ScrollPanel>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
