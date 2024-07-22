import BooleanEditor from '@components/inputs/BooleanEditor';
import { GetMessage } from '@lib/common/intl';
import { propertyExists } from '@lib/common/propertyExists';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { UpdateSettingParameters } from '@lib/smAPI/smapiTypes';
import React from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecord } from '../SettingsUtils';
import { GetLine } from './GetLine';

interface CheckBoxLineProps extends SettingsInterface {}

export function GetCheckBoxLine({ ...props }: CheckBoxLineProps): React.ReactElement {
  const { currentSetting, updateStateAndRequest, updateSettingRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  let value = false;
  if (propertyExists(updateSettingRequest.Parameters, props.field)) {
    value = getRecord<UpdateSettingParameters, boolean>(props.field, updateSettingRequest.Parameters) ?? false;
  } else {
    value = getRecord<UpdateSettingParameters, boolean>(props.field, currentSetting) ?? false;
  }

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        <BooleanEditor
          label={label}
          labelInline
          onChange={(e) => {
            e !== undefined && updateStateAndRequest?.({ [props.field]: e });
          }}
          checked={value}
        />
      </div>
    )
  });
}
