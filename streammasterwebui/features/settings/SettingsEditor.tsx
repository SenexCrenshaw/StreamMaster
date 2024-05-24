import StandardHeader from '@components/StandardHeader';
import ResetButton from '@components/buttons/ResetButton';
import SaveButton from '@components/buttons/SaveButton';
import { GetMessage, isEmptyObject } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';
import { useSMContext } from '@lib/signalr/SMProvider';
import { ScrollPanel } from 'primereact/scrollpanel';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { useUpdateSettingRequest } from '@lib/redux/hooks/updateSettingRequest';
import { SettingDto, UpdateSettingRequest, AuthenticationType } from '@lib/smAPI/smapiTypes';
import { useCurrentSettingRequest } from '@lib/redux/hooks/currentSettingRequest';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';
import { GeneralSettings } from './GeneralSettings';
import { BackupSettings } from './BackupSettings';
import { AuthenticationSettings } from './AuthenticationSettings';
import { SDSettings } from './SDSettings';
import { StreamingSettings } from './StreamingSettings';
import { FilesEPGM3USettings } from './FilesEPGM3USettings';
import { DevelopmentSettings } from './DevelopmentSettings';

export const SettingsEditor = () => {
  const { currentSettingRequest, setCurrentSettingRequest } = useCurrentSettingRequest('CurrentSettingDto');
  const { updateSettingRequest, setUpdateSettingRequest } = useUpdateSettingRequest('UpdateSettingRequest');
  const [originalData, setOriginalData] = useState<SettingDto | null>(null);
  const { isSystemReady, settings } = useSMContext();

  useEffect(() => {
    if (isSystemReady && originalData === null && settings && settings.AuthenticationMethod !== undefined) {
      setOriginalData({ ...settings });
      // setCurrentSettingRequest({ ...settings });
    }
  }, [isSystemReady, originalData, settings]);

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
        setOriginalData(null); // Reset originalData to re-fetch on next load
      })
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {});
  }, [isSaveEnabled, updateSettingRequest, setUpdateSettingRequest]);

  const resetData = useCallback(() => {
    if (originalData) {
      setCurrentSettingRequest({ ...originalData });
    }
  }, [originalData, setCurrentSettingRequest]);

  return (
    <StandardHeader displayName={GetMessage('settings')} icon={<SettingsEditorIcon />}>
      <div className="flex flex-column w-full">
        <ScrollPanel className="w-full" style={{ height: 'calc(100vh - 100px)' }}>
          <div className="flex flex-row justify-content-start align-items-start">
            <div className="w-6 pr-1">
              <GeneralSettings />
            </div>
            <div className="w-6 pl-1">
              <div className="flex flex-column w-full">
                <BackupSettings />
                <AuthenticationSettings />
              </div>
            </div>
          </div>
          <div className="flex flex-row justify-content-start align-items-start">
            <div className="w-6 pr-1">
              <StreamingSettings />
            </div>
            <div className="w-6 pr-1">
              <SDSettings />
            </div>
          </div>

          <div className="flex flex-row justify-content-start align-items-start">
            <div className="w-6 pr-1">
              <FilesEPGM3USettings />
            </div>
            <div className="w-6 pr-1"></div>
          </div>

          <DevelopmentSettings />
        </ScrollPanel>
        <div className="flex mt-2 justify-content-center align-items-end">
          <div className="flex justify-content-center align-items-center gap-1">
            <SaveButton disabled={!isSaveEnabled} onClick={onSave} iconFilled label="Save Settings" />
            <ResetButton disabled={!isSaveEnabled} onClick={resetData} iconFilled label="Reset Settings" />
          </div>
        </div>
      </div>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
