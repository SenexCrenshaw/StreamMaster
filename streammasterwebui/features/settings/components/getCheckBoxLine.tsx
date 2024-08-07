import BooleanEditor from '@components/inputs/BooleanEditor';
import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { UpdateSettingParameters } from '@lib/smAPI/smapiTypes';
import React, { useMemo } from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecord } from '../SettingsUtils';
import { GetLine } from './GetLine';

interface CheckBoxLineProps extends SettingsInterface {}

export function GetCheckBoxLine({ ...props }: CheckBoxLineProps): React.ReactElement {
  const { currentSetting, updateStateAndRequest, updateSettingRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  const getValue = useMemo(() => {
    var value = getRecord<UpdateSettingParameters, boolean>(props.field, updateSettingRequest.Parameters) ?? undefined;

    if (value !== undefined) return value;

    return getRecord<UpdateSettingParameters, boolean>(props.field, currentSetting) ?? false;
  }, [currentSetting, props.field, updateSettingRequest.Parameters]);

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
          checked={getValue}
        />
      </div>
    )
  });
}
