import { GetMessage } from '@lib/common/common';

import { getHelp } from '@lib/locales/help_en';
import { Checkbox } from 'primereact/checkbox';
import React from 'react';
import { UpdateChanges, getRecord } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type CheckBoxLineProps = {
  field: string;
  selectedCurrentSettingDto: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getCheckBoxLine({ field, selectedCurrentSettingDto, onChange }: CheckBoxLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <Checkbox
        checked={selectedCurrentSettingDto ? getRecord<SettingDto, boolean>(field, selectedCurrentSettingDto as SettingDto)! : false}
        className="w-full text-left"
        onChange={(e) => {
          UpdateChanges({ field, selectedCurrentSettingDto, onChange, value: !e.target.value });
        }}
        placeholder={label}
        value={selectedCurrentSettingDto ? getRecord<SettingDto, boolean>(field, selectedCurrentSettingDto) : undefined}
      />
    ),
    help
  });
}
