import BooleanEditor from '@components/inputs/BooleanEditor';
import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import React from 'react';
import { UpdateChanges, getRecord } from '../SettingsUtils';
import { GetLine } from './GetLine'; // Import the GetLine function

type CheckBoxLineProps = {
  field: string;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: SettingDto) => void | undefined;
};

export function getCheckBoxLine({ field, currentSettingRequest, onChange }: CheckBoxLineProps): React.ReactElement {
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);
  Logger.debug('getCheckBoxLine', { field });

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        <BooleanEditor
          label={GetMessage(field)}
          labelInline
          onChange={(e) =>
            UpdateChanges({
              currentSettingRequest,
              field,
              onChange,
              value: e
            })
          }
          checked={currentSettingRequest ? getRecord<SettingDto, boolean>(field, currentSettingRequest as SettingDto)! : false}
        />
      </div>
    )
  });
}
