import { GetMessage } from '@lib/common/common';

import { getHelp } from '@lib/locales/help_en';
import { Password } from 'primereact/password';
import React from 'react';
import { UpdateChanges, getRecordString } from './SettingsUtils';
import { getLine } from './getLine';

type PasswordLineProps = {
  field: string;
  warning?: string | null;
  selectedCurrentSettingDto: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getPasswordLine({ field, warning, selectedCurrentSettingDto, onChange }: PasswordLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);

  return getLine({
    label: `${label}:`,
    value: (
      <span className="w-full">
        <Password
          className="password withpadding w-full text-left"
          feedback={false}
          onChange={(e) => {
            UpdateChanges({ field, selectedCurrentSettingDto, onChange, value: e.target.value });
          }}
          placeholder={label}
          toggleMask
          value={selectedCurrentSettingDto ? getRecordString<SettingDto>(field, selectedCurrentSettingDto) : undefined}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help
  });
}
