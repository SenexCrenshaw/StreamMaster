import { GetMessage } from '@lib/common/common';

import { getHelp } from '@lib/locales/help_en';
import { Dropdown } from 'primereact/dropdown';
import { SelectItem } from 'primereact/selectitem';
import React from 'react';
import { UpdateChanges, getRecordString } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

type DropDownLineProps = {
  field: string;
  options: SelectItem[];
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getDropDownLine({ field, options, currentSettingRequest, onChange }: DropDownLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <Dropdown
        className="bordered-text w-full text-left"
        onChange={(e) => {
          if (isFinite(+e.target.value)) {
            UpdateChanges({ field, currentSettingRequest, onChange, value: +e.target.value });
          } else {
            UpdateChanges({ field, currentSettingRequest, onChange, value: e.target.value });
          }
        }}
        options={options}
        placeholder={label}
        value={currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest)?.toLocaleLowerCase() : undefined}
      />
    ),
    help
  });
}
