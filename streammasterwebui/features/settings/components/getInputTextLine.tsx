import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import React from 'react';
import { UpdateChanges, getRecordString } from '../SettingsUtils';
import { getLine } from './getLine';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import StringEditor from '@components/inputs/StringEditor';

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
      <>
        <StringEditor
          darkBackGround
          disableDebounce
          label={label}
          labelInline
          labelInlineSmall={
            defaultSetting === null || defaultSetting === undefined || defaultSetting === '' || help === null || help === undefined || help !== ''
          }
          onChange={(e) => {
            e && UpdateChanges({ currentSettingRequest, field, onChange, value: e });
          }}
          value={currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest) : undefined}
        />

        {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
      </>
    )
  });
}
