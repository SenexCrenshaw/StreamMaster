import { StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { Dropdown } from 'primereact/dropdown';
import { SelectItem } from 'primereact/selectitem';
import React, { useCallback, useEffect, useMemo, useState } from 'react';

interface StreamingProxyTypeSelectorProperties {
  readonly className?: string;
  readonly label?: string;
  readonly onChange: (value: number) => void;
  readonly resetValue?: string;
  readonly value?: StreamingProxyTypes;
}

const StreamingProxyTypeSelector: React.FC<StreamingProxyTypeSelectorProperties> = ({ className, label, onChange, resetValue, value }) => {
  const [selectedStreamProxytype, setSelectedStreamProxytype] = useState<StreamingProxyTypes>(StreamingProxyTypes.SystemDefault);

  console.log(selectedStreamProxytype);
  useEffect(() => {
    if (selectedStreamProxytype === StreamingProxyTypes.SystemDefault) {
      if (value !== undefined && value !== selectedStreamProxytype) {
        setSelectedStreamProxytype(value);
      }
    }
  }, [selectedStreamProxytype, value]);

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = Object.keys(StreamingProxyTypes)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: StreamingProxyTypes[key as keyof typeof StreamingProxyTypes]
      }));

    return options;
  }, []);

  const internalOnChange = (option: number) => {
    console.log('internalOnChange', option);
    if (option === null || option === undefined) return;
    setSelectedStreamProxytype(option);
    onChange(option);
  };

  const valueTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option.label}</div>;
  }, []);

  return (
    <div className="flex flex-column align-items-start">
      {label && (
        <>
          <label htmlFor="numbereditorbody-inputtext">{label}</label>
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
        value={selectedStreamProxytype}
      />
    </div>
  );
};

StreamingProxyTypeSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(StreamingProxyTypeSelector);
