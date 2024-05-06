import { StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { Dropdown } from 'primereact/dropdown';
import { SelectItem } from 'primereact/selectitem';
import React, { useEffect, useMemo, useState } from 'react';

interface StreamingProxyTypeSelectorProperties {
  readonly className?: string;
  readonly onChange: (value: number) => void;
  readonly resetValue?: string;
  readonly value?: StreamingProxyTypes;
}

const StreamingProxyTypeSelector: React.FC<StreamingProxyTypeSelectorProperties> = ({ className, onChange, resetValue, value }) => {
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

  return (
    <div>
      <Dropdown
        className="sm-streamingproxytypeselector"
        onChange={(e) => internalOnChange(e.value)}
        options={getHandlersOptions}
        optionLabel="label"
        optionValue="value"
        valueTemplate={(option: SelectItem) => option?.label}
        value={selectedStreamProxytype}
      />
    </div>
  );
};

StreamingProxyTypeSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(StreamingProxyTypeSelector);
