import SMDropDown from '@components/sm/SMDropDown';
import { Logger } from '@lib/common/logger';
import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';
import { SetSMChannelProxy } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelProxyRequest } from '@lib/smAPI/smapiTypes';
import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, useCallback, useMemo } from 'react';

interface StreamingProxyTypeSelectorProperties {
  readonly data?: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly label?: string;
  readonly onChange?: (value: string) => void;
}

const StreamingProxyTypeSelector: React.FC<StreamingProxyTypeSelectorProperties> = ({ darkBackGround, data, label, onChange: clientOnChange }) => {
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'StreamingProxyType',
    Id: data?.Id?.toString() ?? ''
  });

  const { data: videoProfiles } = useGetVideoProfiles();

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const DefaultStreamingProxyTypes = ['SystemDefault', 'None', 'StreamMaster'];

    const options = DefaultStreamingProxyTypes.map((type) => ({
      label: type,
      value: type
    }));

    if (videoProfiles) {
      videoProfiles.forEach((profile) => {
        options.push({
          label: profile.ProfileName,
          value: profile.ProfileName
        });
      });
    }
    return options;
  }, [videoProfiles]);

  const onChange = async (option: string) => {
    if (option === null || option === undefined) return;
    // setSelectedStreamProxyType(option);
    if (!data) await onSave(option);
    if (clientOnChange) {
      clientOnChange(option);
    }
  };

  const onSave = useCallback(
    async (option: string) => {
      if (!data?.Id) {
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
    if (!data?.StreamingProxyType) {
      return <div className="text-xs text-container  pl-1">SystemDefault</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{data.StreamingProxyType}</div>
      </div>
    );
  }, [data]);

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  // if (data?.StreamingProxyType === undefined) {
  //   return null;
  // }
  Logger.debug('StreamingProxyTypeSelector', 'StreamingProxyType', data?.StreamingProxyType, data?.StreamingProxyType ?? 'SystemDefault');
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
      value={data?.StreamingProxyType ?? 'SystemDefault'}
      contentWidthSize="2"
    />
  );
};

StreamingProxyTypeSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(StreamingProxyTypeSelector);
