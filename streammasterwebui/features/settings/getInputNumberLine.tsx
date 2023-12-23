import { GetMessage } from '@lib/common/common';
import { SettingDto, UpdateSettingRequest } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { InputNumber } from 'primereact/inputnumber';
import React from 'react';
import { UpdateChanges, getRecord } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type InputNumberLineProps = {
  field: string;
  min?: number | null;
  max?: number | null;
  selectedCurrentSettingDto: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getInputNumberLine({ field, min, max, selectedCurrentSettingDto, onChange }: InputNumberLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  const validatedMax = max === null ? 999 : Math.min(max ?? 999, 999);
  const validatedMin = min === null ? 0 : Math.max(Math.min(min ?? 0, validatedMax - 1), 0);

  return getLine({
    label: `${label}:`,
    value: (
      <InputNumber
        className="withpadding w-full text-left"
        max={validatedMax}
        min={validatedMin}
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
