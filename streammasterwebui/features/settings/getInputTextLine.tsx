import TextInput from '@components/inputs/TextInput';
import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import React from 'react';
import { getRecordString, updateNestedProperty } from './SettingsUtils';
import { getLine } from './getLine';

type InputTextLineProps = {
  field: string;
  warning?: string | null;
  selectCurrentSettingDto: SettingDto | undefined;
  onChange: (newValue: SettingDto) => void | undefined;
};

export function getInputTextLine({ field, warning, selectCurrentSettingDto, onChange }: InputTextLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  return getLine({
    label: `${label}:`,
    value: (
      <span className="w-full">
        <TextInput
          dontValidate
          onChange={(e) => {
            if (selectCurrentSettingDto?.sdSettings === undefined) return;
            const updatedSettingDto = { ...selectCurrentSettingDto, sdSettings: { ...selectCurrentSettingDto.sdSettings } };
            updateNestedProperty(updatedSettingDto, field, e);
            onChange(updatedSettingDto);
          }}
          placeHolder={label}
          showCopy
          value={selectCurrentSettingDto ? getRecordString<SettingDto>(field, selectCurrentSettingDto) : undefined}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help,
    defaultSetting
  });
}
