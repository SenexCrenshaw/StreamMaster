import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { InputNumber } from 'primereact/inputnumber';
import React from 'react';
import { getRecord, updateNestedProperty } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type InputNumberLineProps = {
  field: string;
  max?: number | null;
  selectCurrentSettingDto: SettingDto | undefined;
  onChange: (newValue: SettingDto) => void | undefined;
};

export function getInputNumberLine({ field, max, selectCurrentSettingDto, onChange }: InputNumberLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <InputNumber
        className="withpadding w-full text-left"
        max={max === null ? 64 : max}
        min={0}
        onChange={(e) => {
          if (selectCurrentSettingDto?.sdSettings === undefined) return;
          const updatedSettingDto = { ...selectCurrentSettingDto, sdSettings: { ...selectCurrentSettingDto.sdSettings } };
          updateNestedProperty(updatedSettingDto, field, e.value);
          onChange(updatedSettingDto);
        }}
        placeholder={label}
        showButtons
        size={3}
        value={selectCurrentSettingDto ? getRecord<SettingDto, number>(field, selectCurrentSettingDto) : undefined}
      />
    ),
    help
  });
}
