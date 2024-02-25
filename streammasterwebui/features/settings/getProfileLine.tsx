import TextInput from '@components/inputs/TextInput';
import { GetMessage } from '@lib/common/common';
import { SettingDto, UpdateSettingRequest } from '@lib/iptvApi';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import React from 'react';
import { UpdateChanges, getRecordString } from './SettingsUtils';
import { getLine } from './getLine';

type ProfileLineProps = {
  field: string;
  warning?: string | null;
  selectedCurrentSettingDto: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getProfileLine({ field, warning, selectedCurrentSettingDto, onChange }: ProfileLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  const firstLine = getLine({
    label: `Name:`,
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

  return <>{firstLine}</>;
}
