import SMDropDown from '@components/sm/SMDropDown';
import { Logger } from '@lib/common/logger';
import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';
import { SetSMChannelVideoOutputProfileNameRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { SetSMChannelVideoOutputProfileName } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, useCallback, useEffect, useMemo } from 'react';

interface VideoOutputProfileNameSelectorProperties {
  readonly data?: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly label?: string;
  readonly onChange?: (value: string) => void;
  readonly value?: string;
}

const VideoOutputProfileNameSelector: React.FC<VideoOutputProfileNameSelectorProperties> = ({ darkBackGround, data, label, onChange, value }) => {
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'VideoOutputProfileName',
    Id: data?.Id?.toString() ?? ''
  });

  const { data: videoProfiles } = useGetVideoProfiles();

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = [] as any;
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

  const onSave = useCallback(
    async (option: string) => {
      if (!data?.Id) {
        Logger.warn('No data available for saving', { option });
        return;
      }
      setIsCellLoading(true);
      const request: SetSMChannelVideoOutputProfileNameRequest = {
        SMChannelId: data.Id,
        VideoOutputProfileName: option
      };

      try {
        await SetSMChannelVideoOutputProfileName(request).finally(() => setIsCellLoading(false));
        // Logger.info('Streaming proxy type saved successfully', { request });
      } catch (error) {
        Logger.error('Error saving streaming proxy type', { error, request });
      }
    },
    [data, setIsCellLoading]
  );
  const onIChange = useCallback(
    async (option: string) => {
      if (option === null || option === undefined) return;
      onSave(option);
      // // setSelectedStreamProxyType(option);
      // if (!data) await onSave(option);
      if (onChange) {
        onChange(option);
      }
    },
    [onChange, onSave]
  );

  useEffect(() => {
    if (value !== undefined && value !== data?.VideoOutputProfileName) {
      var found = getHandlersOptions.find((element) => element.value === value);
      if (found) {
        onIChange(value);
      }
    }
  }, [data?.VideoOutputProfileName, getHandlersOptions, onIChange, value]);

  const buttonTemplate = useMemo((): ReactNode => {
    if (data?.IsCustomStream) {
      return <div className="text-xs text-container pl-1">StreamMaster</div>;
    }
    if (!data?.VideoOutputProfileName) {
      return <div className="text-xs text-container pl-1">{value}</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{data.VideoOutputProfileName}</div>
      </div>
    );
  }, [data, value]);

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  if (!data?.VideoOutputProfileName || data.IsCustomStream === true) {
    return <div className="text-xs text-container  pl-1">StreamMaster</div>;
  }

  // if (data?.VideoOutputProfileName === undefined) {
  //   return null;
  // }
  // Logger.debug('VideoOutputProfileNameSelector', 'VideoOutputProfileName', data?.VideoOutputProfileName, data?.VideoOutputProfileName ?? 'SystemDefault');
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
        await onIChange(e.value);
      }}
      title="PROXY"
      propertyToMatch="label"
      value={data?.VideoOutputProfileName ?? value}
      contentWidthSize="2"
    />
  );
};

VideoOutputProfileNameSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(VideoOutputProfileNameSelector);
