import NumberEditor from '@components/inputs/NumberEditor';
import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import React from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecord } from '../SettingsUtils';
import { GetLine } from './GetLine'; // Import the GetLine function

interface InputNumberLineProps extends SettingsInterface {
  min?: number | null;
  max?: number | null;
  showComma?: boolean;
}

export function GetInputNumberLine({ ...props }: InputNumberLineProps): React.ReactElement {
  const { currentSetting, updateStateAndRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  const validatedMax = props.max === null ? 3600 : Math.min(props.max ?? 999999, 999999);
  const validatedMin = props.min === null ? 0 : Math.max(Math.min(props.min ?? 0, validatedMax - 1), 0);

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        <NumberEditor
          darkBackGround
          label={label}
          labelInline
          max={validatedMax}
          min={validatedMin}
          onChange={(e) => {
            e !== undefined && updateStateAndRequest?.({ [props.field]: e });
          }}
          showButtons
          showComma={props.showComma}
          value={currentSetting ? getRecord<SettingDto, number>(props.field, currentSetting) : undefined}
        />
      </div>
    )
  });
}
