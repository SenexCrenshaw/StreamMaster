import { GetMessage } from '@lib/common/intl';
import { getHelp } from '@lib/locales/help_en';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode } from 'react';
import { UpdateChanges, getRecordString } from '../SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';
import { getDefaultSetting } from '@lib/locales/default_setting';
import SMDropDown from '@components/sm/SMDropDown';

type DropDownLineProps = {
  field: string;
  options: SelectItem[];
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getDropDownLine({ field, options, currentSettingRequest, onChange }: DropDownLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  const valueTemplate = (option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  };

  const value = currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest) : undefined;

  const buttonTemplate = (): ReactNode => {
    return <div className="text-container pl-1">{value}</div>;
  };

  return getLine({
    defaultSetting,
    help,
    value: (
      <SMDropDown
        buttonDarkBackground
        label={label}
        buttonTemplate={buttonTemplate()}
        onChange={(e) => {
          const value = isFinite(+e.target.value) ? +e.target.value : e.target.value;
          UpdateChanges({ currentSettingRequest, field, onChange, value });
        }}
        data={options}
        dataKey="label"
        itemTemplate={valueTemplate}
      />
    )
  });
}
