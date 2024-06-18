import SMDropDown from '@components/sm/SMDropDown';
import { getEnumValueByKey } from '@lib/common/enumTools';
import { Logger } from '@lib/common/logger';
import { UpdateFileProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { M3UOutputProfile, M3UOutputProfileRequest, UpdateFileProfileRequest, ValidM3USetting } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';

import { ReactNode, useCallback, useMemo } from 'react';

interface FileProfileValueDropDownProps {
  readonly darkBackGround?: boolean;
  readonly header: string;
  readonly name?: string;
  readonly field?: string;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly value: string;
  onChange?: (value: SelectItem) => void;
}

const FileProfileValueDropDown = ({ ...props }: FileProfileValueDropDownProps) => {
  const update = useCallback((request: UpdateFileProfileRequest) => {
    Logger.debug('FileProfileDropDown', request);

    UpdateFileProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const onChange = (value: SelectItem) => {
    if (props.name && props.field) {
      const m3uOutputProfile = {} as M3UOutputProfileRequest;
      m3uOutputProfile[props.field as keyof M3UOutputProfileRequest] = value.label ?? ('' as M3UOutputProfile[T]);
      const ret = { M3UOutputProfile: m3uOutputProfile, Name: props.name } as UpdateFileProfileRequest;

      update(ret);
      return;
    }

    props.onChange?.(value);
  };

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = Object.keys(ValidM3USetting)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: ValidM3USetting[key as keyof typeof ValidM3USetting]
      }));

    return options;
  }, []);

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  const buttonTemplate = useMemo((): ReactNode => {
    return (
      <div className="sm-epg-selector">
        <div className="text-container " style={{ paddingLeft: '0.12rem' }}>
          {props.value}
        </div>
      </div>
    );
  }, [props.value]);

  return (
    <SMDropDown
      buttonDarkBackground={props.darkBackGround}
      buttonTemplate={buttonTemplate}
      contentWidthSize="2"
      data={getHandlersOptions}
      dataKey="label"
      info=""
      itemTemplate={itemTemplate}
      label={props.label}
      onChange={onChange}
      optionValue="label"
      scrollHeight="20vh"
      title={props.header + ' Mapping'}
      value={{ id: getEnumValueByKey(ValidM3USetting, props.value as keyof typeof ValidM3USetting), label: props.value }}
    />
  );
};

export default FileProfileValueDropDown;
