import StringEditor from '@components/inputs/StringEditor';
import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import React from 'react';
import { UpdateChanges, getRecordString } from '../SettingsUtils';
import { getLine } from './getLine';

type InputTextLineProps = {
  field: string;
  warning?: string | null;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: SettingDto) => void | undefined;
};

export function getInputTextLine({ field, warning, currentSettingRequest, onChange }: InputTextLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  const value = getRecordString<SettingDto>(field, currentSettingRequest);
  Logger.debug('getInputTextLine', { defaultSetting, field, value });

  return getLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-8">
        <StringEditor
          darkBackGround
          disableDebounce
          label={label}
          labelInline
          labelInlineSmall={
            defaultSetting === null || defaultSetting === undefined || defaultSetting === '' || help === null || help === undefined || help !== ''
          }
          onChange={(e) => {
            e !== undefined && UpdateChanges({ currentSettingRequest, field, onChange, value: e });
          }}
          value={currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest) : undefined}
        />

        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </div>
    )
  });
}
