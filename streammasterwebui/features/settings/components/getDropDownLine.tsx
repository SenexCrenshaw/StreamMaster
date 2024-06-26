import SMDropDown from '@components/sm/SMDropDown';
import { GetMessage } from '@lib/common/intl';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode } from 'react';
import { UpdateChanges, getRecordString } from '../SettingsUtils';
import { GetLine } from './GetLine'; // Import the GetLine function

type DropDownLineProps = {
  field: string;
  options: SelectItem[];
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: SettingDto) => void | undefined;
};

export function GetDropDownLine({ field, options, currentSettingRequest, onChange }: DropDownLineProps): React.ReactElement {
  const label = GetMessage(field);
  const help = getHelp(field);
  const defaultSetting = getDefaultSetting(field);

  const valueTemplate = (option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  };

  const value = currentSettingRequest ? getRecordString<SettingDto>(field, currentSettingRequest) : undefined;

  const buttonTemplate = (): ReactNode => {
    return <div className="text-container pl-1">{value}</div>;
  };

  return GetLine({
    defaultSetting,
    help,
    value: (
      <div className="sm-w-12">
        <SMDropDown
          buttonDarkBackground
          buttonTemplate={buttonTemplate()}
          contentWidthSize="2"
          data={options}
          dataKey="label"
          itemTemplate={valueTemplate}
          info=""
          label={label}
          labelInline
          onChange={(e) => {
            const value = isFinite(+e.value) ? +e.value : e.value;
            UpdateChanges({ currentSettingRequest, field, onChange, value });
          }}
          hasCloseButton
          showClose={false}
          title="Proxy"
        />
      </div>
    )
  });
}
