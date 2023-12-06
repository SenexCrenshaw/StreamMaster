import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { InputNumber } from 'primereact/inputnumber';
import React from 'react';
import { getRecord } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type InputNumberLineProps = {
  field: string;
  max?: number | null;
  newData: SettingDto;
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>;
};

export function InputNumberLine({ field, max, newData, setNewData }: InputNumberLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <InputNumber
        className="withpadding w-full text-left"
        max={max === null ? 64 : max}
        min={0}
        onValueChange={(e) => setNewData({ ...newData, [field]: e.target.value })}
        placeholder={label}
        showButtons
        size={3}
        value={getRecord<SettingDto, number>(field, newData)}
      />
    ),
    help
  });
}
