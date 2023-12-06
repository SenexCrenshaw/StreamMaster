import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import React, { useMemo } from 'react';
// Import the getLine function
import { AuthenticationType } from '@lib/common/streammaster_enums';
import useSettings from '@lib/useSettings';
import { Button } from 'primereact/button';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import { getDropDownLine } from './getDropDownLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';

type AuthenticationSettingsProps = {
  newData: SettingDto; // Adjust the type accordingly
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>; // Adjust the type accordingly
};

export function AuthenticationSettings({ newData, setNewData }: AuthenticationSettingsProps): React.ReactElement {
  const setting = useSettings();

  const adminUserNameError = useMemo((): string | undefined => {
    if (newData.authenticationMethod === AuthenticationType.Forms && newData.adminUserName === '') return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [newData.adminUserName, newData.authenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (newData.authenticationMethod === AuthenticationType.Forms && newData.adminPassword === '') return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [newData.adminPassword, newData.authenticationMethod]);

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

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('authentication')}>
      {getInputTextLine({ field: 'apiKey', newData, setNewData })}
      {getDropDownLine({ field: 'authenticationMethod', options: getAuthTypeOptions(), newData, setNewData })}
      {getInputTextLine({ field: 'adminUserName', warning: adminUserNameError, newData, setNewData })}
      {getPasswordLine({ field: 'adminPassword', warning: adminPasswordError, newData, setNewData })}
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
