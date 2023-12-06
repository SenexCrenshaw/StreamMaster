import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { Checkbox } from 'primereact/checkbox';
import React from 'react';
import { getRecord } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type CheckBoxLineProps = {
  field: string;
  newData: SettingDto; // Adjust the type accordingly
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>; // Adjust the type accordingly
};

export function getCheckBoxLine({ field, newData, setNewData }: CheckBoxLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <Checkbox
        checked={getRecord<SettingDto, boolean>(field, newData) ?? false}
        className="w-full text-left"
        onChange={(e) => setNewData({ ...newData, [field]: !e.target.value })}
        placeholder={label}
        value={getRecord<SettingDto, boolean>(field, newData)}
      />
    ),
    help
  });
}
