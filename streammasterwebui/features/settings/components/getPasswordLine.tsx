import { GetMessage } from '@lib/common/intl';
import { Logger } from '@lib/common/logger';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { UpdateSettingParameters } from '@lib/smAPI/smapiTypes';
import { Password } from 'primereact/password';
import React, { useMemo } from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecordString } from '../SettingsUtils';
import { GetLine } from './GetLine';

interface PasswordLineProps extends SettingsInterface {
  readonly autoFocus?: boolean;
  readonly labelInline?: boolean;
}

export function GetPasswordLine({ ...props }: PasswordLineProps): React.ReactElement {
  const { currentSetting, updateSettingRequest, updateStateAndRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  const getDiv = useMemo(() => {
    let ret = 'flex justify-content-center';

    if (label && !props.labelInline) {
      ret += ' flex-column';
    }

    if (props.labelInline) {
      ret += ' align-items-start';
    } else {
      ret += ' align-items-center';
    }

    return ret;
  }, [label, props.labelInline]);

  const inputGetDiv = useMemo(() => {
    let ret = 'sm-input';

    if (props.labelInline) {
      ret += ' w-12';
    }

    return ret;
  }, [props.labelInline]);

  const getValue = useMemo(() => {
    var value = getRecordString<UpdateSettingParameters>(props.field, updateSettingRequest.Parameters) ?? undefined;
    // if (propertyExists(updateSettingRequest.Parameters, props.field)) {
    //   return getRecordString<UpdateSettingParameters>(props.field, updateSettingRequest.Parameters) ?? '';
    // }

    if (value !== undefined) return value;

    return getRecordString<UpdateSettingParameters>(props.field, currentSetting) ?? '';
  }, [currentSetting, props.field, updateSettingRequest.Parameters]);

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        {label && !props.labelInline && (
          <>
            <label className="pl-15">{label.toUpperCase()}</label>
          </>
        )}
        <div className={getDiv}>
          {label && props.labelInline && <div className={props.labelInline ? 'w-4' : 'w-6'}>{label.toUpperCase()}</div>}
          <Password
            className={inputGetDiv}
            feedback
            onChange={(e) => {
              e !== undefined && updateStateAndRequest?.({ [props.field]: e.target.value });
            }}
            toggleMask
            value={getValue}
          />
          <br />
          {props.warning !== null && props.warning !== undefined && <span className="text-xs text-orange-500">{props.warning}</span>}
        </div>
      </div>
    )
  });
}
