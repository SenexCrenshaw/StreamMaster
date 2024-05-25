import React from 'react';
import { getLine } from './getLine'; // Import the getLine function
import { UpdateChanges, getRecord } from '../SettingsUtils';
import { GetMessage } from '@lib/common/intl';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';
import BooleanEditor from '@components/inputs/BooleanEditor';
import { Logger } from '@lib/common/logger';
import { getDefaultSetting } from '@lib/locales/default_setting';

type CheckBoxLineProps = {
  field: string;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
};

export function getCheckBoxLine({ field, currentSettingRequest, onChange }: CheckBoxLineProps): React.ReactElement {
  // const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);
  Logger.debug('getCheckBoxLine', { field });

  return getLine({
    defaultSetting,
    help,
    value: (
      <BooleanEditor
        label={GetMessage(field)}
        labelInline
        onChange={(e) => UpdateChanges({ currentSettingRequest, field, onChange, value: !e })}
        checked={currentSettingRequest ? getRecord<SettingDto, boolean>(field, currentSettingRequest as SettingDto)! : false}
      />
    )
  });
}
