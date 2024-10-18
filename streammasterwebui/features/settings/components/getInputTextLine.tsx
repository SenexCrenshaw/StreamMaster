import StringEditor from '@components/inputs/StringEditor';
import { GetMessage } from '@lib/common/intl';
import { propertyExists } from '@lib/common/propertyExists';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { UpdateSettingParameters } from '@lib/smAPI/smapiTypes';
import React, { useMemo } from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecordString } from '../SettingsUtils';
import { GetLine } from './GetLine';

interface InputTextLineProps extends SettingsInterface {}

export function GetInputTextLine({ ...props }: InputTextLineProps): React.ReactElement {
  const { currentSetting, updateSettingRequest, updateStateAndRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  const getValue = useMemo(() => {
    if (propertyExists(updateSettingRequest.Parameters, props.field)) {
      return getRecordString<UpdateSettingParameters>(props.field, updateSettingRequest.Parameters) ?? '';
    }

    return getRecordString<UpdateSettingParameters>(props.field, currentSetting) ?? '';
  }, [currentSetting, props.field, updateSettingRequest.Parameters]);

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        <StringEditor
          darkBackGround
          disableDebounce
          label={label}
          labelInline
          onChange={(e) => {
            e !== undefined && updateStateAndRequest?.({ [props.field]: e });
          }}
          value={getValue}
        />

        {props.warning !== null && props.warning !== undefined && <span className="text-xs text-orange-500">{props.warning}</span>}
      </div>
    )
  });
}
