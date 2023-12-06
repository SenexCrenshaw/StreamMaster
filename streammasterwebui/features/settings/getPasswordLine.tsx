import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getHelp } from '@lib/locales/help_en';
import { Password } from 'primereact/password';
import React from 'react';
import { getRecordString, updateNestedProperty } from './SettingsUtils';
import { getLine } from './getLine';

type PasswordLineProps = {
  field: string;
  warning?: string | null;
  selectCurrentSettingDto: SettingDto | undefined;
  onChange: (newValue: SettingDto) => void | undefined;
};

export function getPasswordLine({ field, warning, selectCurrentSettingDto, onChange }: PasswordLineProps): React.ReactElement {
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
            if (selectCurrentSettingDto?.sdSettings === undefined) return;
            const updatedSettingDto = { ...selectCurrentSettingDto, sdSettings: { ...selectCurrentSettingDto.sdSettings } };
            updateNestedProperty(updatedSettingDto, field, e.target.value);
            onChange(updatedSettingDto);
          }}
          placeholder={label}
          toggleMask
          value={selectCurrentSettingDto ? getRecordString<SettingDto>(field, selectCurrentSettingDto) : undefined}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help
  });
}
