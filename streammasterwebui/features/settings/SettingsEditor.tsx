import StandardHeader from '@components/StandardHeader';
import ResetButton from '@components/buttons/ResetButton';
import SaveButton from '@components/buttons/SaveButton';
import { GetMessage, isEmptyObject } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';

import { ScrollPanel } from 'primereact/scrollpanel';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { AuthenticationSettings } from './AuthenticationSettings';
import { BackupSettings } from './BackupSettings';
import { DevelopmentSettings } from './DevelopmentSettings';
import { FilesEPGM3USettings } from './FilesEPGM3USettings';
import { GeneralSettings } from './GeneralSettings';
import { ProfileSettings } from './ProfileSettings';
import { SDSettings } from './SDSettings';
import { StreamingSettings } from './StreamingSettings';
import { useUpdateSettingRequest } from '@lib/redux/hooks/updateSettingRequest';
import { SettingDto, UpdateSettingRequest, AuthenticationType } from '@lib/smAPI/smapiTypes';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { useCurrentSettingRequest } from '@lib/redux/hooks/currentSettingRequest';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';

export const SettingsEditor = () => {
  const { currentSettingRequest, setCurrentSettingRequest } = useCurrentSettingRequest('CurrentSettingDto');
  const { updateSettingRequest, setUpdateSettingRequest } = useUpdateSettingRequest('UpdateSettingRequest');

  const [originalData, setOriginalData] = useState<SettingDto>({} as SettingDto);

  const settingsQuery = useGetSettings();

  useEffect(() => {
    if (settingsQuery.isLoading || !settingsQuery.data) return;

    setCurrentSettingRequest({ ...settingsQuery.data });
    setUpdateSettingRequest({} as UpdateSettingRequest);
    setOriginalData({ ...settingsQuery.data });
  }, [setCurrentSettingRequest, setUpdateSettingRequest, settingsQuery.data, settingsQuery.isLoading]);

  const adminUserNameError = useMemo((): string | undefined => {
    if (currentSettingRequest?.AuthenticationMethod === AuthenticationType.Forms && currentSettingRequest?.AdminUserName === '')
      return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [currentSettingRequest?.AdminUserName, currentSettingRequest?.AuthenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (currentSettingRequest?.AuthenticationMethod === AuthenticationType.Forms && currentSettingRequest?.AdminPassword === '')
      return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [currentSettingRequest?.AdminPassword, currentSettingRequest?.AuthenticationMethod]);

  const isSaveEnabled = useMemo((): boolean => {
    if (currentSettingRequest?.EnableSSL === true && currentSettingRequest?.SSLCertPath === '') {
      console.log('enableSSL');
      return false;
    }

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
      console.log('adminUserNameError');
      return false;
    }

    if (isEmptyObject(updateSettingRequest)) {
      return false;
    }

    return true;
  }, [currentSettingRequest, updateSettingRequest, adminUserNameError, adminPasswordError]);

  const onSave = useCallback(() => {
    if (!isSaveEnabled || !updateSettingRequest) {
      return;
    }

    UpdateSetting(updateSettingRequest)
      .then(() => {
        const reset: UpdateSettingRequest = {};
        setUpdateSettingRequest(reset);
      })
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {});
  }, [isSaveEnabled, updateSettingRequest, setUpdateSettingRequest]);

  const resetData = useCallback(() => {
    setCurrentSettingRequest({ ...originalData });
  }, [originalData, setCurrentSettingRequest]);

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
          <div className="flex justify-content-center align-items-center gap-1">
            <SaveButton disabled={!isSaveEnabled} onClick={() => onSave()} iconFilled label="Save Settings" />
            <ResetButton disabled={!isSaveEnabled} onClick={() => resetData()} iconFilled label="Reset Settings" />
          </div>
        </div>
      </div>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
