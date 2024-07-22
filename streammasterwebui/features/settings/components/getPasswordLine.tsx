import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { Password } from 'primereact/password';
import React from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecordString } from '../SettingsUtils';
import { GetLine } from './GetLine';

interface PasswordLineProps extends SettingsInterface {
  readonly autoFocus?: boolean;
  readonly labelInline?: boolean;
}

export function GetPasswordLine({ ...props }: PasswordLineProps): React.ReactElement {
  const { currentSetting, updateStateAndRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        {label && !props.labelInline && (
          <>
            <label className="pl-15">{label.toUpperCase()}</label>
            <div className="pt-small" />
          </>
        )}
        <div className={`flex ${props.labelInline ? 'align-items-center' : 'flex-column align-items-start'}`}>
          {label && props.labelInline && <div className={props.labelInline ? 'w-4' : 'w-6'}>{label.toUpperCase()}</div>}
          <Password
            className="text-container sm-input"
            feedback
            onChange={(e) => {
              e !== undefined && updateStateAndRequest?.({ [props.field]: e });
            }}
            toggleMask
            value={currentSetting ? getRecordString<SettingDto>(props.field, currentSetting) : undefined}
          />
          <br />
          {props.warning !== null && props.warning !== undefined && <span className="text-xs text-orange-500">{props.warning}</span>}
        </div>
      </div>
    )
  });
}
