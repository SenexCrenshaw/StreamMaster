import TextInput from '@components/inputs/TextInput';
import { GetMessage } from '@lib/common/common';

import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import React from 'react';
import { UpdateChanges, getRecordString } from './SettingsUtils';
import { getLine } from './getLine';
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

type InputTextLineProps = {
  field: string;
  warning?: string | null;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getInputTextLine({ field, warning, currentSettingRequest, onChange }: InputTextLineProps): React.ReactElement {
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
            UpdateChanges({ field, currentSettingRequest, onChange, value: e });
          }}
          placeHolder={label}
          showCopy
          value={currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest) : undefined}
        />
        <br />
        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </span>
    ),
    help,
    defaultSetting
  });
}
