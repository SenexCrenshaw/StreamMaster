import SMDropDown from '@components/sm/SMDropDown';
import { isNumber } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import { SetSMChannelProxy } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelProxyRequest, StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, useCallback, useMemo } from 'react';

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
    <SMDropDown
      buttonLabel="PROXY"
      buttonDarkBackground={darkBackGround}
      buttonTemplate={buttonTemplate}
      data={getHandlersOptions}
      dataKey="label"
      filter
      filterBy="label"
      itemTemplate={valueTemplate}
      label={label}
      onChange={async (e: any) => {
        await onChange(e.value);
      }}
      title="PROXY"
      optionValue="label"
      value={data?.StreamingProxyType}
      contentWidthSize="2"
    />
  );
};

StreamingProxyTypeSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(StreamingProxyTypeSelector);
