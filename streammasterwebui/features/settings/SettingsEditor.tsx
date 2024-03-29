import StandardHeader from '@components/StandardHeader';
import ResetButton from '@components/buttons/ResetButton';
import SaveButton from '@components/buttons/SaveButton';
import { GetMessage, isEmptyObject } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';
import { AuthenticationType } from '@lib/common/streammaster_enums';

import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import { useSelectUpdateSettingRequest } from '@lib/redux/slices/selectedUpdateSettingRequestSlice';

import { ScrollPanel } from 'primereact/scrollpanel';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { AuthenticationSettings } from './AuthenticationSettings';
import { BackupSettings } from './BackupSettings';
import { DevelopmentSettings } from './DevelopmentSettings';
import { FilesEPGM3USettings } from './FilesEPGM3USettings';
import { GeneralSettings } from './GeneralSettings';
// import { ProfileSettings } from './ProfileSettings';
import { ProfileSettings } from './ProfileSettings';
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
  }, [setSelectedCurrentSettingDto, setSelectedUpdateSettingRequest, settingsQuery]);

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
    if (selectedCurrentSettingDto?.enableSSL === true && selectedCurrentSettingDto?.sslCertPath === '') {
      console.log('enableSSL');
      return false;
    }

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
      console.log('adminUserNameError');
      return false;
    }

    if (isEmptyObject(selectUpdateSettingRequest)) {
      return false;
    }

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
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {});
  }, [isSaveEnabled, selectUpdateSettingRequest, setSelectedUpdateSettingRequest]);

  const resetData = useCallback(() => {
    setSelectedCurrentSettingDto({ ...originalData });
  }, [originalData, setSelectedCurrentSettingDto]);

  return (
    <StandardHeader displayName={GetMessage('settings')} icon={<SettingsEditorIcon />}>
      <div className="flex flex-column">
        <ScrollPanel style={{ height: 'calc(100vh - 100px)', width: '100%' }}>
          <GeneralSettings />

          <BackupSettings />

          <AuthenticationSettings />

          <StreamingSettings />

          {settingsQuery.data?.hls?.hlsM3U8Enable && <ProfileSettings />}

          <SDSettings />

          <FilesEPGM3USettings />

          <DevelopmentSettings />
        </ScrollPanel>
        <div className="flex mt-2 justify-content-center align-items-end">
          <div className="flex justify-content-center align-items-center gap-2">
            <SaveButton disabled={!isSaveEnabled} onClick={() => onSave()} iconFilled label="Save Settings" />
            <ResetButton disabled={!isSaveEnabled} onClick={() => resetData()} iconFilled label="Reset Settings" />
          </div>
        </div>
      </div>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
