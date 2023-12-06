import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { Checkbox } from 'primereact/checkbox';
import React from 'react';
import { getRecord, updateNestedProperty } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type CheckBoxLineProps = {
  field: string;
  selectCurrentSettingDto: SettingDto | undefined;
  onChange: (newValue: SettingDto) => void | undefined;
};

export function getCheckBoxLine({ field, selectCurrentSettingDto, onChange }: CheckBoxLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <Checkbox
        checked={selectCurrentSettingDto ? getRecord<SettingDto, boolean>(field, selectCurrentSettingDto as SettingDto)! : false}
        className="w-full text-left"
        onChange={(e) => {
          if (selectCurrentSettingDto?.sdSettings === undefined) return;
          const updatedSettingDto = { ...selectCurrentSettingDto, sdSettings: { ...selectCurrentSettingDto.sdSettings } };
          updateNestedProperty(updatedSettingDto, field, !e.target.value);
          onChange(updatedSettingDto);
        }}
        placeholder={label}
        value={selectCurrentSettingDto ? getRecord<SettingDto, boolean>(field, selectCurrentSettingDto) : undefined}
      />
    ),
    help
  });
}
