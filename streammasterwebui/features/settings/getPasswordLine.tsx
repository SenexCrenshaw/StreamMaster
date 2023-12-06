import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { Password } from 'primereact/password';
import React from 'react';
import { getRecordString } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type PasswordLineProps = {
  field: string;
  warning?: string | null;
  newData: SettingDto; // Adjust the type accordingly
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>; // Adjust the type accordingly
};

export function getPasswordLine({ field, warning, newData, setNewData }: PasswordLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <span className="w-full">
        <Password
          className="password withpadding w-full text-left"
          feedback={false}
          onChange={(e) => setNewData({ ...newData, [field]: e.target.value })}
          placeholder={label}
          toggleMask
          value={getRecordString<SettingDto>(field, newData)}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help
  });
}
