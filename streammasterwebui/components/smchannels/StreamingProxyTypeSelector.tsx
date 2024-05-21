import { SMOverlay } from '@components/sm/SMOverlay';
import { SMChannelDto, StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, Suspense, lazy, useCallback, useEffect, useMemo, useState } from 'react';
const SMScroller = lazy(() => import('@components/sm/SMScroller'));

interface StreamingProxyTypeSelectorProperties {
  readonly data?: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly label?: string;
  readonly onChange?: (value: number) => void;
}

const StreamingProxyTypeSelector: React.FC<StreamingProxyTypeSelectorProperties> = ({ darkBackGround, data, label, onChange: clientOnChange }) => {
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

  const onChange = (option: number) => {
    if (option === null || option === undefined) return;
    setSelectedStreamProxyType(option);
    clientOnChange && clientOnChange(option);
  };

  const buttonTemplate = useMemo((): ReactNode => {
    if (!data) {
      return <div className="text-xs text-container text-white-alpha-40">None</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{data.StreamingProxyType}</div>
      </div>
    );
  }, [data]);

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
      <div className={darkBackGround ? 'sm-input-border-dark w-full' : 'w-full'}>
        <SMOverlay title="PROXY" widthSize="2" icon="pi-chevron-down" buttonTemplate={buttonTemplate} buttonLabel="EPG">
          <div className="flex flex-row w-12 sm-card border-radius-left border-radius-right ">
            <Suspense>
              <div className="flex w-12 ml-1">
                <SMScroller
                  data={getHandlersOptions}
                  dataKey="label"
                  filter
                  filterBy="label"
                  itemSize={26}
                  itemTemplate={valueTemplate}
                  onChange={(e) => {
                    onChange(e.value);
                  }}
                  scrollHeight={150}
                  value={selectedStreamProxyType}
                />
              </div>
            </Suspense>
          </div>
        </SMOverlay>
      </div>
    </div>
  );
};

StreamingProxyTypeSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(StreamingProxyTypeSelector);
