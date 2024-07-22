import SMButton from '@components/sm/SMButton';
import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { useSMContext } from '@lib/context/SMProvider';
import { AuthenticationType } from '@lib/smAPI/smapiTypes';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React, { useMemo } from 'react';
import { BaseSettings } from './BaseSettings';
import { GetDropDownLine } from './components/GetDropDownLine';
import { GetInputTextLine } from './components/GetInputTextLine';
import { GetPasswordLine } from './components/GetPasswordLine';

export function AuthenticationSettings(): React.ReactElement {
  const { settings } = useSMContext();
  const { currentSetting } = useSettingsContext();

  const adminUserNameError = useMemo((): string | undefined => {
    if (currentSetting?.AuthenticationMethod !== 'None' && currentSetting?.AdminUserName === '') return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [currentSetting]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (currentSetting?.AuthenticationMethod !== 'None' && currentSetting?.AdminPassword === '') return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [currentSetting]);

  const getAuthTypeOptions = (): SelectItem[] => {
    const options = Object.keys(AuthenticationType)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: AuthenticationType[key as keyof typeof AuthenticationType]
      }));

    return options;
  };

  if (currentSetting === null || currentSetting === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="AUTHENTICATION">
      <>
        {/* {getInputTextLine({  field: 'ApiKey', onChange })} */}
        {GetDropDownLine({ field: 'AuthenticationMethod', options: getAuthTypeOptions() })}
        {GetInputTextLine({ field: 'AdminUserName', warning: adminUserNameError })}
        {GetPasswordLine({ field: 'AdminPassword', warning: adminPasswordError, labelInline: true })}
        <div className="flex w-12 settings-line justify-content-end align-items-center">
          <div className="w-3">
            <SMButton
              buttonDisabled={!settings.AuthenticationMethod || settings.AuthenticationMethod === 'None'}
              icon="pi-check"
              label={GetMessage('signout')}
              onClick={() => (window.location.href = '/logout')}
              rounded
              iconFilled
              buttonClassName="icon-green"
            />
          </div>
        </div>
      </>
    </BaseSettings>
  );
}
