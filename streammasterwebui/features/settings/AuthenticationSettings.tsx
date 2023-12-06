import { GetMessage } from '@lib/common/common';
import React, { useMemo } from 'react';
// Import the getLine function
import { AuthenticationType } from '@lib/common/streammaster_enums';
import { SettingDto } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import useSettings from '@lib/useSettings';
import { Button } from 'primereact/button';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import { getDropDownLine } from './getDropDownLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';

export function AuthenticationSettings(): React.ReactElement {
  const setting = useSettings();
  const { selectCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');

  const adminUserNameError = useMemo((): string | undefined => {
    if (selectCurrentSettingDto?.authenticationMethod === AuthenticationType.Forms && selectCurrentSettingDto?.adminUserName === '')
      return GetMessage('formsAuthRequiresAdminUserName');

    return undefined;
  }, [selectCurrentSettingDto?.adminUserName, selectCurrentSettingDto?.authenticationMethod]);

  const adminPasswordError = useMemo((): string | undefined => {
    if (selectCurrentSettingDto?.authenticationMethod === AuthenticationType.Forms && selectCurrentSettingDto?.adminPassword === '')
      return GetMessage('formsAuthRequiresAdminPassword');

    return undefined;
  }, [selectCurrentSettingDto?.adminPassword, selectCurrentSettingDto?.authenticationMethod]);

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

  const onChange = (newValue: SettingDto) => {
    if (selectCurrentSettingDto === undefined || setSelectedCurrentSettingDto === undefined || newValue === null || newValue === undefined) return;
    setSelectedCurrentSettingDto(newValue);
  };

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('authentication')}>
      {getInputTextLine({ field: 'apiKey', selectCurrentSettingDto, onChange })}
      {getDropDownLine({ field: 'authenticationMethod', options: getAuthTypeOptions(), selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'adminUserName', warning: adminUserNameError, selectCurrentSettingDto, onChange })}
      {getPasswordLine({ field: 'adminPassword', warning: adminPasswordError, selectCurrentSettingDto, onChange })}
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
