import StandardHeader from '@components/StandardHeader';
import { GetMessage } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';
import { AuthenticationType } from '@lib/common/streammaster_enums';
import { SettingDto, useSettingsGetSettingQuery } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
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
  const { selectCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');

  // const [, setNewData] = useState<SettingDto>({} as SettingDto);

  const [originalData, setOriginalData] = useState<SettingDto>({} as SettingDto);

  const settingsQuery = useSettingsGetSettingQuery();

  useEffect(() => {
    if (settingsQuery.isLoading || !settingsQuery.data) return;

    setSelectedCurrentSettingDto({ ...settingsQuery.data });
    setOriginalData({ ...settingsQuery.data });
  }, [setSelectedCurrentSettingDto, settingsQuery]);

  const adminUserNameError = useMemo((): string | undefined => {
    if (selectCurrentSettingDto?.authenticationMethod === AuthenticationType.Forms && selectCurrentSettingDto?.adminUserName === '')
      return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [selectCurrentSettingDto?.adminUserName, selectCurrentSettingDto?.authenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (selectCurrentSettingDto?.authenticationMethod === AuthenticationType.Forms && selectCurrentSettingDto?.adminPassword === '')
      return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [selectCurrentSettingDto?.adminPassword, selectCurrentSettingDto?.authenticationMethod]);

  const isSaveEnabled = useMemo((): boolean => {
    if (JSON.stringify(selectCurrentSettingDto) === JSON.stringify(originalData)) return false;

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
      return false;
    }

    if (selectCurrentSettingDto?.enableSSL === true && selectCurrentSettingDto?.sslCertPath === '') {
      return false;
    }

    return true;
  }, [adminPasswordError, adminUserNameError, selectCurrentSettingDto, originalData]);

  const onSave = useCallback(() => {
    if (!isSaveEnabled || !selectCurrentSettingDto) {
      return;
    }

    UpdateSetting(selectCurrentSettingDto)
      .then(() => {})
      .catch(() => {});
  }, [isSaveEnabled, selectCurrentSettingDto]);

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
        setSelectedCurrentSettingDto({ ...originalData });
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

        <GeneralSettings />

        <AuthenticationSettings />

        <StreamingSettings />

        <SDSettings />

        <FilesEPGM3USettings />

        <DevelopmentSettings />
      </ScrollPanel>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
