import { GetMessage } from '@lib/common/common';

import { getHelp } from '@lib/locales/help_en';
import { Checkbox } from 'primereact/checkbox';
import React from 'react';
import { UpdateChanges, getRecord } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

type CheckBoxLineProps = {
  field: string;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getCheckBoxLine({ field, currentSettingRequest, onChange }: CheckBoxLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <Checkbox
        checked={currentSettingRequest ? getRecord<SettingDto, boolean>(field, currentSettingRequest as SettingDto)! : false}
        className="w-full text-left"
        onChange={(e) => {
          UpdateChanges({ field, currentSettingRequest, onChange, value: !e.target.value });
        }}
        placeholder={label}
        value={currentSettingRequest ? getRecord<SettingDto, boolean>(field, currentSettingRequest) : undefined}
      />
    ),
    help
  });
}
