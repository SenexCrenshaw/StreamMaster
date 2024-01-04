import { StreamingProxyTypes as StreamingProxyTypesEnum } from '@lib/common/streammaster_enums';
import { Dropdown } from 'primereact/dropdown';
import { SelectItem } from 'primereact/selectitem';
import React, { useEffect, useState } from 'react';

interface StreamingProxyTypeSelectorProperties {
  readonly className?: string;
  readonly onChange: (value: number) => void;
  readonly resetValue?: string;
  readonly value?: StreamingProxyTypesEnum;
}

const StreamingProxyTypeSelector: React.FC<StreamingProxyTypeSelectorProperties> = ({ className, onChange, resetValue, value }) => {
  const [selectedStreamProxytype, setSelectedStreamProxytype] = useState<StreamingProxyTypesEnum | undefined>(undefined);

  useEffect(() => {
    if (selectedStreamProxytype === undefined && value !== undefined) {
      setSelectedStreamProxytype(value);
    }
  }, [selectedStreamProxytype, value]);

  const getHandlersOptions = (): SelectItem[] => {
    const test = Object.entries(StreamingProxyTypesEnum)
      .splice(0, Object.keys(StreamingProxyTypesEnum).length / 2)
      .map(
        ([number, word]) =>
          ({
            label: word,
            value: number
          } as SelectItem)
      );

    return test;
  };

  const internalOnChange = (option: number) => {
    console.log('internalOnChange', option);
    if (option === null || option === undefined) return;
    setSelectedStreamProxytype(option);
    onChange(option);
  };

  return (
    <div className="flex w-full">
      <Dropdown
        className={`w-full ${className}`}
        onChange={(e) => internalOnChange(e.value)}
        options={getHandlersOptions()}
        value={selectedStreamProxytype === undefined ? '0' : selectedStreamProxytype.toString()}
      />
    </div>
  );
};

StreamingProxyTypeSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(StreamingProxyTypeSelector);
