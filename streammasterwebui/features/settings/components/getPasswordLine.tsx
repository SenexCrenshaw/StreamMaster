import { GetMessage } from '@lib/common/intl';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { Password } from 'primereact/password';
import React from 'react';
import { UpdateChanges, getRecordString } from '../SettingsUtils';
import { GetLine } from './GetLine';

type PasswordLineProps = {
  onChange: (existing: SettingDto, updatedValues: SettingDto) => void | undefined;
  readonly autoFocus?: boolean;
  readonly currentSettingRequest: SettingDto;
  readonly field: string;
  readonly labelInline?: boolean;
  readonly labelInlineSmall?: boolean;
  readonly warning?: string | null;
};

export function getPasswordLine({
  currentSettingRequest,
  field,
  labelInline = true,
  labelInlineSmall = true,
  onChange,
  warning
}: PasswordLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        {label && !labelInline && (
          <>
            <label className="pl-15">{label.toUpperCase()}</label>
            <div className="pt-small" />
          </>
        )}
        <div className={`flex ${labelInline ? 'align-items-center' : 'flex-column align-items-start'}`}>
          {label && labelInline && <div className={labelInline ? 'w-4' : 'w-6'}>{label.toUpperCase()}</div>}
          <Password
            className={labelInlineSmall ? 'w-12' : 'w-6'}
            feedback
            onChange={(e) => {
              UpdateChanges({ currentSettingRequest, field, onChange, value: e.target.value });
            }}
            toggleMask
            value={currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest) : undefined}
          />
          <br />
          {warning !== null && warning !== undefined && <span className="text-xs text-orange-500">{warning}</span>}
        </div>
      </div>
    )
  });
}
