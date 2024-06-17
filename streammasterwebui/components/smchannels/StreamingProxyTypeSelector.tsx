import SMDropDown from '@components/sm/SMDropDown';
import { isNumber } from '@lib/common/common';
import { Logger } from '@lib/common/logger';
import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
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
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'StreamingProxyType',
    Id: data?.Id.toString() ?? ''
  });

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = Object.keys(StreamingProxyTypes)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: StreamingProxyTypes[key as keyof typeof StreamingProxyTypes]
      }));

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
      setIsCellLoading(true);
      const request: SetSMChannelProxyRequest = {
        SMChannelId: data.Id,
        StreamingProxy: option
      };

      try {
        await SetSMChannelProxy(request).finally(() => setIsCellLoading(false));
        Logger.info('Streaming proxy type saved successfully', { request });
      } catch (error) {
        Logger.error('Error saving streaming proxy type', { error, request });
      }
    },
    [data, setIsCellLoading]
  );

  const buttonTemplate = useMemo((): ReactNode => {
    if (!data) {
      return <div className="text-xs text-container text-white-alpha-40 pl-1">None</div>;
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

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
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
      buttonIsLoading={isCellLoading}
      itemTemplate={itemTemplate}
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
