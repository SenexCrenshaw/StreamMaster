import TextInput from '@components/inputs/TextInput';
import { GetMessage } from '@lib/common/common';

import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import React from 'react';
import { UpdateChanges, getRecordString } from './SettingsUtils';
import { getLine } from './getLine';

type InputTextLineProps = {
  field: string;
  warning?: string | null;
  selectedCurrentSettingDto: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getInputTextLine({ field, warning, selectedCurrentSettingDto, onChange }: InputTextLineProps): React.ReactElement {
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
            UpdateChanges({ field, selectedCurrentSettingDto, onChange, value: e });
          }}
          placeHolder={label}
          showCopy
          value={selectedCurrentSettingDto ? getRecordString<SettingDto>(field, selectedCurrentSettingDto) : undefined}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help,
    defaultSetting
  });
}
