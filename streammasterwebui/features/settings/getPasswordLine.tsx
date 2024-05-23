import { GetMessage } from '@lib/common/common';

import { getHelp } from '@lib/locales/help_en';
import { Password } from 'primereact/password';
import React from 'react';
import { UpdateChanges, getRecordString } from './SettingsUtils';
import { getLine } from './getLine';
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

type PasswordLineProps = {
  field: string;
  warning?: string | null;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getPasswordLine({ field, warning, currentSettingRequest, onChange }: PasswordLineProps): React.ReactElement {
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
            UpdateChanges({ field, currentSettingRequest, onChange, value: e.target.value });
          }}
          placeholder={label}
          toggleMask
          value={currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest) : undefined}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help
  });
}
