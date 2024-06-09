import StandardHeader from '@components/StandardHeader';
import ResetButton from '@components/buttons/ResetButton';
import SaveButton from '@components/buttons/SaveButton';
import { isEmptyObject } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';
import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import { useCurrentSettingRequest } from '@lib/redux/hooks/currentSettingRequest';
import { useUpdateSettingRequest } from '@lib/redux/hooks/updateSettingRequest';
import { useSMContext } from '@lib/signalr/SMProvider';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';
import { AuthenticationType, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';
import { ScrollPanel } from 'primereact/scrollpanel';
import { memo, useCallback, useEffect, useMemo } from 'react';
import { AuthenticationSettings } from './AuthenticationSettings';
import { BackupSettings } from './BackupSettings';
import { DevelopmentSettings } from './DevelopmentSettings';
import { FilesEPGM3USettings } from './FilesEPGM3USettings';
import { GeneralSettings } from './GeneralSettings';
import { SDSettings } from './SDSettings';
import { StreamingSettings } from './StreamingSettings';

export const SettingsEditor = () => {
  const { currentSettingRequest, setCurrentSettingRequest } = useCurrentSettingRequest('CurrentSettingDto');
  const { updateSettingRequest, setUpdateSettingRequest } = useUpdateSettingRequest('UpdateSettingRequest');
  const { isSystemReady, settings } = useSMContext();

  useEffect(() => {
    if (!isSystemReady) return;

    if (currentSettingRequest.ApiKey === undefined) {
      Logger.info('SettingsEditor', settings);
      setCurrentSettingRequest({ ...settings });
      setUpdateSettingRequest({} as UpdateSettingRequest);
    }
  }, [isSystemReady, settings, currentSettingRequest, setCurrentSettingRequest, setUpdateSettingRequest]);

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
      return false;
    }

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
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
        setUpdateSettingRequest({} as UpdateSettingRequest);
      })
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {});
  }, [isSaveEnabled, updateSettingRequest, setUpdateSettingRequest]);

  const resetData = useCallback(() => {
    setCurrentSettingRequest({ ...settings });
  }, [setCurrentSettingRequest, settings]);

  if (!isSystemReady || settings === undefined) {
    return <div>Loading</div>;
  }

  return (
    <StandardHeader displayName={GetMessage('settings')} icon={<SettingsEditorIcon />}>
      <div className="flex flex-column w-full">
        <ScrollPanel className="w-full" style={{ height: 'calc(100vh - 100px)' }}>
          <div className="flex flex-row justify-content-start align-items-start">
            <div className="w-6 pr-1 flex flex-column gap-3">
              <GeneralSettings />
              <StreamingSettings />
              <FilesEPGM3USettings />
              <BackupSettings />
            </div>
            <div className="w-6 pl-1 flex flex-column gap-3">
              <AuthenticationSettings />
              <StreamingSettings />
              <SDSettings />
            </div>
          </div>
          <div className="w-12 pl-1 pt-3">
            <DevelopmentSettings />
          </div>
        </ScrollPanel>
        <div className="flex mt-2 justify-content-center align-items-end">
          <div className="flex justify-content-center align-items-center gap-1">
            <SaveButton disabled={!isSaveEnabled} onClick={onSave} iconFilled label="Save" />
            <ResetButton disabled={!isSaveEnabled} onClick={resetData} iconFilled label="Reset" />
          </div>
        </div>
      </div>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
