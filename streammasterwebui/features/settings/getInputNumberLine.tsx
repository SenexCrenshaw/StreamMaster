import { GetMessage } from '@lib/common/common';

import { getHelp } from '@lib/locales/help_en';
import { InputNumber } from 'primereact/inputnumber';
import React from 'react';
import { UpdateChanges, getRecord } from './SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

type InputNumberLineProps = {
  field: string;
  min?: number | null;
  max?: number | null;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getInputNumberLine({ field, min, max, currentSettingRequest, onChange }: InputNumberLineProps): React.ReactElement {
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
          UpdateChanges({ field, currentSettingRequest, onChange, value: e.value });
        }}
        placeholder={label}
        showButtons
        size={3}
        value={currentSettingRequest ? getRecord<SettingDto, number>(field, currentSettingRequest) : undefined}
      />
    ),
    help
  });
}
