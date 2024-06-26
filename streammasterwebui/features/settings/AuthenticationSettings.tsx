import SMButton from '@components/sm/SMButton';
import { SMCard } from '@components/sm/SMCard';
import { GetMessage } from '@lib/common/intl';
import { useSMContext } from '@lib/signalr/SMProvider';
import { AuthenticationType } from '@lib/smAPI/smapiTypes';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React, { useMemo } from 'react';
import { GetDropDownLine } from './components/getDropDownLine';
import { getInputTextLine } from './components/getInputTextLine';
import { getPasswordLine } from './components/getPasswordLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';

export function AuthenticationSettings(): React.ReactElement {
  const { settings } = useSMContext();
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

  const adminUserNameError = useMemo((): string | undefined => {
    if (currentSettingRequest?.AuthenticationMethod === AuthenticationType.Forms && currentSettingRequest?.AdminUserName === '')
      return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [currentSettingRequest]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (currentSettingRequest?.AuthenticationMethod === AuthenticationType.Forms && currentSettingRequest?.AdminPassword === '')
      return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [currentSettingRequest]);

  const getAuthTypeOptions = (): SelectItem[] => {
    const options = Object.keys(AuthenticationType)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: AuthenticationType[key as keyof typeof AuthenticationType]
      }));

    return options;
  };

  if (currentSettingRequest === null || currentSettingRequest === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <SMCard
      info=""
      hasCloseButton
      darkBackGround={false}
      title="AUTHENTICATION"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          <div className="layout-padding-bottom" />
          <div className="settings-lines ">
            {getInputTextLine({ currentSettingRequest, field: 'ApiKey', onChange })}
            {GetDropDownLine({ currentSettingRequest, field: 'AuthenticationMethod', onChange, options: getAuthTypeOptions() })}
            {getInputTextLine({ currentSettingRequest, field: 'AdminUserName', onChange, warning: adminUserNameError })}
            {getPasswordLine({ currentSettingRequest, field: 'AdminPassword', onChange, warning: adminPasswordError })}
            <div className="flex w-12 settings-line justify-content-end align-items-center">
              <div className="w-2 text-right pr-2">{GetMessage('signout')}</div>
              <div className="w-2">
                <SMButton
                  buttonDisabled={!settings.AuthenticationMethod || (settings.AuthenticationMethod as number) === 0}
                  icon="pi-check"
                  label={GetMessage('signout')}
                  onClick={() => (window.location.href = '/logout')}
                  rounded
                  iconFilled
                  buttonClassName="icon-green"
                />
              </div>
            </div>
          </div>
        </div>
        <div className="layout-padding-bottom" />
      </div>
    </SMCard>
  );
}
