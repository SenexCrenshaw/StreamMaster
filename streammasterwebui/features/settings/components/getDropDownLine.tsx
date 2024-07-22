import SMDropDown from '@components/sm/SMDropDown';
import { GetMessage } from '@lib/common/intl';
import { propertyExists } from '@lib/common/propertyExists';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { getDefaultSetting } from '@lib/locales/default_setting';
import { getHelp } from '@lib/locales/help_en';
import { UpdateSettingParameters } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, useMemo } from 'react';
import { SettingsInterface } from '../SettingsInterface';
import { getRecordString } from '../SettingsUtils';
import { GetLine } from './GetLine';

interface DropDownLineProps extends SettingsInterface {
  options: SelectItem[];
}

export function GetDropDownLine({ ...props }: DropDownLineProps): React.ReactElement {
  const { currentSetting, updateStateAndRequest, updateSettingRequest } = useSettingsContext();
  const label = GetMessage(props.field);
  const help = getHelp(props.field);
  const defaultSetting = getDefaultSetting(props.field);

  const getValue = useMemo(() => {
    if (propertyExists(updateSettingRequest.Parameters, props.field)) {
      return getRecordString<UpdateSettingParameters>(props.field, updateSettingRequest.Parameters) ?? '';
    }

    return getRecordString<UpdateSettingParameters>(props.field, currentSetting) ?? '';
  }, [currentSetting, props.field, updateSettingRequest.Parameters]);

  const valueTemplate = (option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  };

  const buttonTemplate = (): ReactNode => {
    var found = props.options.find((o) => o.label?.toLowerCase() === getValue.toLowerCase());
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
            updateStateAndRequest({ [props.field]: e.label });
          }}
          title={label}
          value={getValue}
        />
      </div>
    )
  });
}
