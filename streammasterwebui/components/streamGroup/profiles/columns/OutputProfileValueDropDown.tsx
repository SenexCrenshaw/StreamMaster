import SMDropDown from '@components/sm/SMDropDown';
import { getEnumValueByKey } from '@lib/common/enumTools';
import { Logger } from '@lib/common/logger';
import { UpdateOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';

import { UpdateOutputProfileRequest, ValidM3USetting } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';

import { ReactNode, useCallback, useMemo } from 'react';

interface OutputProfileValueDropDownProps {
  readonly darkBackGround?: boolean;
  readonly header: string;
  readonly name: string;
  readonly field?: string;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly value: string;
  onChange?: (value: SelectItem) => void;
}

const OutputProfileValueDropDown = ({ ...props }: OutputProfileValueDropDownProps) => {
  const update = useCallback(
    (request: UpdateOutputProfileRequest) => {
      Logger.debug('OutputProfileValueDropDown', request);
      request.ProfileName = props.name;

      UpdateOutputProfile(request)
        .then((res) => {})
        .catch((error) => {
          console.log('error', error);
        })
        .finally();
    },
    [props.name]
  );

  const onChange = (value: SelectItem) => {
    if (props.name && props.field) {
      const outputProfile = {} as UpdateOutputProfileRequest;
      outputProfile[props.field as keyof UpdateOutputProfileRequest] = value.label ?? ('' as UpdateOutputProfileRequest[T]);
      const ret = outputProfile;

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

export default OutputProfileValueDropDown;