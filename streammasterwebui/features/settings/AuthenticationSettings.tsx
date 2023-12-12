import { GetMessage } from '@lib/common/common';
import { AuthenticationType } from '@lib/common/streammaster_enums';
import useSettings from '@lib/useSettings';
import { Button } from 'primereact/button';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import React, { useMemo } from 'react';
import { getDropDownLine } from './getDropDownLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';

export function AuthenticationSettings(): React.ReactElement {
  const setting = useSettings();
  const { onChange, selectedCurrentSettingDto } = useSettingChangeHandler();

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

  if (selectedCurrentSettingDto === null || selectedCurrentSettingDto === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('authentication')} toggleable>
      {getInputTextLine({ field: 'apiKey', selectedCurrentSettingDto, onChange })}
      {getDropDownLine({ field: 'authenticationMethod', options: getAuthTypeOptions(), selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'adminUserName', warning: adminUserNameError, selectedCurrentSettingDto, onChange })}
      {getPasswordLine({ field: 'adminPassword', warning: adminPasswordError, selectedCurrentSettingDto, onChange })}
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
