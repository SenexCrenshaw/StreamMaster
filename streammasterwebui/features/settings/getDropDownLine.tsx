import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { Dropdown } from 'primereact/dropdown';
import { SelectItem } from 'primereact/selectitem';
import React from 'react';
import { getRecordString } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type DropDownLineProps = {
  field: string;
  options: SelectItem[];
  newData: SettingDto; // Adjust the type accordingly
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>; // Adjust the type accordingly
};

export function getDropDownLine({ field, options, newData, setNewData }: DropDownLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <Dropdown
        className="withpadding w-full text-left"
        onChange={(e) => setNewData({ ...newData, [field]: Number.parseInt(e.target.value) })}
        options={options}
        placeholder={label}
        value={getRecordString<SettingDto>(field, newData)}
      />
    ),
    help
  });
}
