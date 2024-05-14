import { SMChannelDto, StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { Dropdown } from 'primereact/dropdown';
import { SelectItem } from 'primereact/selectitem';
import React, { useCallback, useEffect, useMemo, useState } from 'react';

interface StreamingProxyTypeSelectorProperties {
  readonly data?: SMChannelDto;
  readonly className?: string;
  readonly label?: string;
  readonly onChange?: (value: number) => void;
  readonly resetValue?: string;
}

const StreamingProxyTypeSelector: React.FC<StreamingProxyTypeSelectorProperties> = ({ className, data, label, onChange, resetValue }) => {
  const [selectedStreamProxyType, setSelectedStreamProxyType] = useState<StreamingProxyTypes | undefined>(undefined);

  const getEnumValueByKey = (key: string): StreamingProxyTypes | undefined => {
    return (StreamingProxyTypes as any)[key];
  };

  useEffect(() => {
    if (selectedStreamProxyType === undefined) {
      if (data !== undefined && data.StreamingProxyType !== undefined) {
        const enumValue = getEnumValueByKey(data.StreamingProxyType as unknown as string);
        setSelectedStreamProxyType(enumValue);
      }
    }
  }, [data, selectedStreamProxyType]);

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = Object.keys(StreamingProxyTypes)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: StreamingProxyTypes[key as keyof typeof StreamingProxyTypes]
      }));
    console.log('options', options);
    return options;
  }, []);

  const internalOnChange = (option: number) => {
    if (option === null || option === undefined) return;
    setSelectedStreamProxyType(option);
    onChange && onChange(option);
  };

  const valueTemplate = useCallback(
    (option: SelectItem): JSX.Element => {
      console.log(data, option);
      if (option === null || option === undefined) {
        return <div className="text-xs text-container"></div>;
      }
      return <div className="text-xs text-container">{option.label}</div>;
    },
    [data]
  );

  if (selectedStreamProxyType === undefined && data === undefined) {
    return null;
  }

  return (
    <div className="flex flex-column align-items-start">
      {label && (
        <>
          <label className="pl-15" htmlFor="numbereditorbody-inputtext">
            {label.toUpperCase()}
          </label>
          <div className="pt-small" />
        </>
      )}
      <Dropdown
        className="sm-streamingproxy-selector"
        onChange={(e) => internalOnChange(e.value)}
        options={getHandlersOptions}
        optionLabel="label"
        optionValue="value"
        valueTemplate={(option: SelectItem) => valueTemplate(option)}
        value={selectedStreamProxyType}
      />
    </div>
  );
};

StreamingProxyTypeSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(StreamingProxyTypeSelector);
