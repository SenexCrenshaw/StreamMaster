import StandardHeader from '@components/StandardHeader';
import { GetMessage, isEmptyObject } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';
import { AuthenticationType } from '@lib/common/streammaster_enums';
import { SettingDto, UpdateSettingRequest, useSettingsGetSettingQuery } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import { useSelectUpdateSettingRequest } from '@lib/redux/slices/selectedUpdateSettingRequestSlice';
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
  const { selectedCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');
  const { selectUpdateSettingRequest, setSelectedUpdateSettingRequest } = useSelectUpdateSettingRequest('UpdateSettingRequest');

  const [originalData, setOriginalData] = useState<SettingDto>({} as SettingDto);

  const settingsQuery = useSettingsGetSettingQuery();

  useEffect(() => {
    if (settingsQuery.isLoading || !settingsQuery.data) return;

    setSelectedCurrentSettingDto({ ...settingsQuery.data });
    setSelectedUpdateSettingRequest({} as UpdateSettingRequest);
    setOriginalData({ ...settingsQuery.data });
  }, [setSelectedCurrentSettingDto, settingsQuery]);

  const adminUserNameError = useMemo((): string | undefined => {
    if (selectedCurrentSettingDto?.authenticationMethod === AuthenticationType.Forms && selectedCurrentSettingDto?.adminUserName === '')
      return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [selectedCurrentSettingDto?.adminUserName, selectedCurrentSettingDto?.authenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (selectedCurrentSettingDto?.authenticationMethod === AuthenticationType.Forms && selectedCurrentSettingDto?.adminPassword === '')
      return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [selectedCurrentSettingDto?.adminPassword, selectedCurrentSettingDto?.authenticationMethod]);

  const isSaveEnabled = useMemo((): boolean => {
    console.log('Start', isEmptyObject(selectUpdateSettingRequest), selectUpdateSettingRequest);

    if (selectedCurrentSettingDto?.enableSSL === true && selectedCurrentSettingDto?.sslCertPath === '') {
      console.log('enableSSL');
      return false;
    }

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
      console.log('adminUserNameError');
      return false;
    }

    if (isEmptyObject(selectUpdateSettingRequest)) {
      console.log('selectUpdateSettingRequest', selectUpdateSettingRequest);
      return false;
    }

    // if (JSON.stringify(selectedCurrentSettingDto) === JSON.stringify(originalData)) {
    //   console.log('equals');
    //   return false;
    // }

    console.log(true);
    return true;
  }, [adminPasswordError, adminUserNameError, selectUpdateSettingRequest, selectedCurrentSettingDto?.enableSSL, selectedCurrentSettingDto?.sslCertPath]);

  const onSave = useCallback(() => {
    if (!isSaveEnabled || !selectUpdateSettingRequest) {
      return;
    }

    UpdateSetting(selectUpdateSettingRequest)
      .then(() => {
        const reset: UpdateSettingRequest = {};
        setSelectedUpdateSettingRequest(reset);
      })
      .catch(() => {})
      .finally(() => {});
  }, [isSaveEnabled, selectUpdateSettingRequest, setSelectedUpdateSettingRequest]);

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
