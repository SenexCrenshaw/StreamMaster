import StandardHeader from '@components/StandardHeader';
import ResetButton from '@components/buttons/ResetButton';
import SaveButton from '@components/buttons/SaveButton';
import { isEmptyObject } from '@lib/common/common';
import { SettingsEditorIcon } from '@lib/common/icons';
import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import { useSMContext } from '@lib/context/SMProvider';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { UpdateSettingRequest } from '@lib/smAPI/smapiTypes';
import { ScrollPanel } from 'primereact/scrollpanel';
import { memo, useCallback, useMemo } from 'react';
import { AuthenticationSettings } from './AuthenticationSettings';
import { BackupSettings } from './BackupSettings';
import { DevelopmentSettings } from './DevelopmentSettings';
import { GeneralSettings } from './GeneralSettings';
import { MiscSettings } from './MiscSettings';
import { SDSettings } from './SDSettings';
import { StreamingSettings } from './StreamingSettings';
import LogsDialog from './LogsDialog';

const SettingsEditor = () => {
  const { currentSetting, setCurrentSetting, updateSettingRequest, setUpdateSettingRequest } = useSettingsContext();
  const { isSystemReady, settings } = useSMContext();
  const getSettings = useGetSettings();

  Logger.debug('SettingsEditor', 'currentSetting', currentSetting);
  const adminUserNameError = useMemo((): string | undefined => {
    if (currentSetting?.AuthenticationMethod !== 'None' && currentSetting?.AdminUserName === '') return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [currentSetting?.AdminUserName, currentSetting?.AuthenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (currentSetting?.AuthenticationMethod !== 'None' && currentSetting?.AdminPassword === '') return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [currentSetting?.AdminPassword, currentSetting?.AuthenticationMethod]);

  const isSaveEnabled = useMemo((): boolean => {
    if (currentSetting?.EnableSSL === true && currentSetting?.SSLCertPath === '') {
      return false;
    }

    if (adminUserNameError !== undefined || adminPasswordError !== undefined) {
      return false;
    }

    if (isEmptyObject(updateSettingRequest)) {
      return false;
    }

    return true;
  }, [currentSetting, updateSettingRequest, adminUserNameError, adminPasswordError]);

  const onSave = useCallback(() => {
    if (!isSaveEnabled || !updateSettingRequest) {
      return;
    }

    UpdateSetting(updateSettingRequest)
      .then(() => {
        //
      })
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        var t = {
          ...currentSetting,
          ...updateSettingRequest.Parameters
        };

        var sdSettings = {
          ...currentSetting.SDSettings,
          ...updateSettingRequest.Parameters.SDSettings
        };

        t.SDSettings = sdSettings;

        Logger.debug('SettingsEditor', 'Updating settings', t);
        setCurrentSetting(t);
        setUpdateSettingRequest({} as UpdateSettingRequest);
        getSettings.SetIsForced(true);
      });
  }, [isSaveEnabled, updateSettingRequest, setCurrentSetting, currentSetting, setUpdateSettingRequest, getSettings]);

  const resetData = useCallback(() => {
    Logger.debug('SettingsEditor', 'Resetting data', settings.DeviceID);
    setUpdateSettingRequest({} as UpdateSettingRequest);
    // setCurrentSetting({ ...settings });
    getSettings.SetIsForced(true);
  }, [getSettings, setUpdateSettingRequest, settings.DeviceID]);

  if (!isSystemReady) {
    //|| settings === undefined || !propertyExists(currentSetting, 'DeviceID')) {
    return <div>Loading</div>;
  }

  return (
    <StandardHeader displayName={GetMessage('settings')} icon={<SettingsEditorIcon />}>
      <div className="flex flex-column w-full">
        <ScrollPanel className="w-full" style={{ height: 'calc(100vh - 80px)' }}>
          <div className="flex flex-row justify-content-start align-items-start">
            <div className="w-4 pr-1 flex flex-column gap-3">
              <GeneralSettings />
              <AuthenticationSettings />
              <DevelopmentSettings />
              <LogsDialog />
            </div>
            <div className="w-4 pl-1 flex flex-column gap-3">
              <SDSettings />
            </div>
            <div className="w-4 pl-1 flex flex-column gap-3">
              <StreamingSettings />
              <BackupSettings />
              <MiscSettings />
            </div>
          </div>
        </ScrollPanel>
        <div className="flex mt-2 justify-content-center align-items-end">
          <div className="sm-w-5rem">
            <SaveButton buttonDisabled={!isSaveEnabled} onClick={onSave} iconFilled label="Save" />
          </div>
          <div className="sm-w-5rem">
            <ResetButton buttonDisabled={!isSaveEnabled} onClick={resetData} iconFilled label="Reset" />
          </div>
        </div>
      </div>
    </StandardHeader>
  );
};

export default memo(SettingsEditor);
