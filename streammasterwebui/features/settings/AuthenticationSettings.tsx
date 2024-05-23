import { GetMessage } from '@lib/common/common';
import useSettings from '@lib/useSettings';
import { Button } from 'primereact/button';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React, { useMemo } from 'react';
import { getDropDownLine } from './getDropDownLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';
import { AuthenticationType } from '@lib/smAPI/smapiTypes';

export function AuthenticationSettings(): React.ReactElement {
  const setting = useSettings();
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
    const test = Object.entries(AuthenticationType)
      .splice(0, Object.keys(AuthenticationType).length / 2)
      .map(
        ([number, word]) =>
          ({
            label: word,
            value: number
          } as SelectItem)
      );

    return test;
  };

  if (currentSettingRequest === null || currentSettingRequest === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('authentication')} toggleable>
      {getInputTextLine({ field: 'apiKey', currentSettingRequest, onChange })}
      {getDropDownLine({ field: 'authenticationMethod', options: getAuthTypeOptions(), currentSettingRequest, onChange })}
      {getInputTextLine({ field: 'adminUserName', warning: adminUserNameError, currentSettingRequest, onChange })}
      {getPasswordLine({ field: 'adminPassword', warning: adminPasswordError, currentSettingRequest, onChange })}
      <div className="flex col-12">
        <div className="flex col-2 col-offset-1">
          <span>{GetMessage('signout')}</span>
        </div>
        <div className="flex col-3 m-0 p-0 debug">
          <Button
            disabled={!setting.authenticationType || (setting.authenticationType as number) === 0}
            icon="pi pi-check"
            label={GetMessage('signout')}
            onClick={() => (window.location.href = '/logout')}
            rounded
            severity="success"
            size="small"
          />
        </div>
      </div>
    </Fieldset>
  );
}
