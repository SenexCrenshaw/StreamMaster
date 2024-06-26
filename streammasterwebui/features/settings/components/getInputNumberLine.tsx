import NumberEditor from '@components/inputs/NumberEditor';
import { GetMessage } from '@lib/common/intl';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import React from 'react';
import { UpdateChanges, getRecord } from '../SettingsUtils';
import { getLine } from './getLine'; // Import the getLine function

type InputNumberLineProps = {
  field: string;
  min?: number | null;
  max?: number | null;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: SettingDto) => void | undefined;
};

export function getInputNumberLine({ field, min, max, currentSettingRequest, onChange }: InputNumberLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  const validatedMax = max === null ? 999 : Math.min(max ?? 999, 999);
  const validatedMin = min === null ? 0 : Math.max(Math.min(min ?? 0, validatedMax - 1), 0);

  return getLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-8">
        <NumberEditor
          darkBackGround
          label={label}
          labelInline
          max={validatedMax}
          min={validatedMin}
          onChange={(e) => {
            UpdateChanges({ currentSettingRequest, field, onChange, value: e });
          }}
          showButtons
          value={currentSettingRequest ? getRecord<SettingDto, number>(field, currentSettingRequest) : undefined}
        />
      </div>
    )
  });
}
