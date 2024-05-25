import React, { ReactNode, Suspense, lazy, useCallback, useMemo } from 'react';
import { SelectItem } from 'primereact/selectitem';
import SMOverlay from '@components/sm/SMOverlay';
import { SetSMChannelProxy } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelProxyRequest, StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { Logger } from '@lib/common/logger';
import { isNumber } from '@lib/common/common';

const SMScroller = lazy(() => import('@components/sm/SMScroller'));

interface StreamingProxyTypeSelectorProperties {
  readonly data?: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly label?: string;
  readonly onChange?: (value: number) => void;
}

const StreamingProxyTypeSelector: React.FC<StreamingProxyTypeSelectorProperties> = ({ darkBackGround, data, label, onChange: clientOnChange }) => {
  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = Object.keys(StreamingProxyTypes)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: StreamingProxyTypes[key as keyof typeof StreamingProxyTypes]
      }));

    Logger.debug('Handler options generated', { options });
    return options;
  }, []);

  const getEnumKeyByValue = useCallback(<T,>(enumObj: T, value: number): string | null => {
    const entries = Object.entries(enumObj as unknown as Record<string, number>);
    for (const [key, val] of entries) {
      if (val === value) {
        return key;
      }
    }
    return null;
  }, []);

  const onChange = async (option: number) => {
    if (option === null || option === undefined) return;
    // setSelectedStreamProxyType(option);
    await onSave(option);
    if (clientOnChange) {
      clientOnChange(option);
    }
  };

  const onSave = useCallback(
    async (option: number) => {
      if (!data) {
        Logger.warn('No data available for saving', { option });
        return;
      }

      const request: SetSMChannelProxyRequest = {
        SMChannelId: data.Id,
        StreamingProxy: option
      };

      try {
        await SetSMChannelProxy(request);
        Logger.info('Streaming proxy type saved successfully', { request });
      } catch (error) {
        Logger.error('Error saving streaming proxy type', { error, request });
      }
    },
    [data]
  );

  const buttonTemplate = useMemo((): ReactNode => {
    if (!data) {
      return <div className="text-xs text-container text-white-alpha-40">None</div>;
    }

    if (isNumber(data.StreamingProxyType)) {
      const enumKey = getEnumKeyByValue(StreamingProxyTypes, data.StreamingProxyType);
      return (
        <div className="sm-epg-selector">
          <div className="text-container pl-1">{enumKey}</div>
        </div>
      );
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{data.StreamingProxyType}</div>
      </div>
    );
  }, [data, getEnumKeyByValue]);

  const valueTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  if (data?.StreamingProxyType === undefined && data === undefined) {
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
          <div className="flex flex-row w-12 sm-card border-radius-left border-radius-right">
            <Suspense fallback={<div>Loading...</div>}>
              <div className="flex w-12">
                <SMScroller
                  data={getHandlersOptions}
                  dataKey="label"
                  filter
                  filterBy="label"
                  itemSize={26}
                  itemTemplate={valueTemplate}
                  onChange={async (e) => {
                    await onChange(e.value);
                  }}
                  scrollHeight={150}
                  value={data?.StreamingProxyType}
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
