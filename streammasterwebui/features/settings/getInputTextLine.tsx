import TextInput from '@components/inputs/TextInput';
import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import React from 'react';
import { getRecordString } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type InputTextLineProps = {
  field: string;
  warning?: string | null;
  newData: SettingDto; // Adjust the type accordingly
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>; // Adjust the type accordingly
};

export function getInputTextLine({ field, warning, newData, setNewData }: InputTextLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  return getLine({
    label: `${label}:`,
    value: (
      <span className="w-full">
        <TextInput
          dontValidate
          onChange={(e) => setNewData({ ...newData, [field]: e })}
          placeHolder={label}
          showCopy
          value={getRecordString<SettingDto>(field, newData)}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help,
    defaultSetting
  });
}
