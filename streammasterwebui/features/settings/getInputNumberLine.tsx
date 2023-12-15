import { GetMessage } from '@lib/common/common';
import { SettingDto, UpdateSettingRequest } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { InputNumber } from 'primereact/inputnumber';
import React from 'react';
import { UpdateChanges, getRecord } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type InputNumberLineProps = {
  field: string;
  max?: number | null;
  selectedCurrentSettingDto: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getInputNumberLine({ field, max, selectedCurrentSettingDto, onChange }: InputNumberLineProps): React.ReactElement {
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
          UpdateChanges({ field, selectedCurrentSettingDto, onChange, value: e.value });
        }}
        placeholder={label}
        showButtons
        size={3}
        value={selectedCurrentSettingDto ? getRecord<SettingDto, number>(field, selectedCurrentSettingDto) : undefined}
      />
    ),
    help
  });
}
