import SMDropDown from '@components/sm/SMDropDown';
import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode } from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecordString } from '../SettingsUtils';
import { GetLine } from './GetLine';

interface DropDownLineProps extends SettingsInterface {
  options: SelectItem[];
}

export function GetDropDownLine({ ...props }: DropDownLineProps): React.ReactElement {
  const { currentSettingRequest, updateStateAndRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  const valueTemplate = (option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  };

  const value = currentSettingRequest ? getRecordString<SettingDto>(props.field, currentSettingRequest) : undefined;

  const buttonTemplate = (): ReactNode => {
    var found = props.options.find((o) => o.label === value);
    return <div className="text-container pl-1">{found?.label ?? 'NA'}</div>;
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
          data={props.options}
          dataKey="label"
          itemTemplate={valueTemplate}
          info=""
          label={label}
          labelInline
          onChange={(e) => {
            updateStateAndRequest?.({ [props.field]: e.label });
            // var found = props.options.find((o) => o.label === e.label);
            // found !== undefined && props.updateStateAndRequest?.({ [props.field]: found.label });
          }}
          title={label}
          value={value}
        />
      </div>
    )
  });
}
