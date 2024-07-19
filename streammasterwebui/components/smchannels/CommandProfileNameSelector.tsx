import SMDropDown from '@components/sm/SMDropDown';
import { Logger } from '@lib/common/logger';
import useIsCellLoading from '@lib/redux/hooks/useIsCellLoading';
import useGetCommandProfiles from '@lib/smAPI/Profiles/useGetCommandProfiles';
import { SetSMChannelCommandProfileNameRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { SetSMChannelCommandProfileName } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { SelectItem } from 'primereact/selectitem';
import React, { ReactNode, useCallback, useEffect, useMemo } from 'react';

interface CommandProfileNameSelectorProperties {
  readonly data?: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly label?: string;
  readonly onChange?: (value: string) => void;
  readonly value?: string;
}

const CommandProfileNameSelector: React.FC<CommandProfileNameSelectorProperties> = ({ darkBackGround, data, label, onChange, value }) => {
  const [isCellLoading, setIsCellLoading] = useIsCellLoading({
    Entity: 'SMChannel',
    Field: 'CommandProfileName',
    Id: data?.Id?.toString() ?? ''
  });

  const { data: CommandProfiles } = useGetCommandProfiles();

  const getHandlersOptions = useMemo((): SelectItem[] => {
    const options = [] as any;
    if (CommandProfiles) {
      CommandProfiles.forEach((profile) => {
        options.push({
          label: profile.ProfileName,
          value: profile.ProfileName
        });
      });
    }
    return options;
  }, [CommandProfiles]);

  const onSave = useCallback(
    async (option: string) => {
      if (!data?.Id) {
        Logger.warn('No data available for saving', { option });
        return;
      }
      setIsCellLoading(true);
      const request: SetSMChannelCommandProfileNameRequest = {
        SMChannelId: data.Id,
        CommandProfileName: option
      };

      try {
        await SetSMChannelCommandProfileName(request).finally(() => setIsCellLoading(false));
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
    if (value !== undefined && value !== data?.CommandProfileName) {
      var found = getHandlersOptions.find((element) => element.value === value);
      if (found) {
        onIChange(value);
      }
    }
  }, [data?.CommandProfileName, getHandlersOptions, onIChange, value]);

  const buttonTemplate = useMemo((): ReactNode => {
    if (data?.IsCustomStream) {
      return <div className="text-xs text-container pl-1">StreamMaster</div>;
    }
    if (!data?.CommandProfileName) {
      return <div className="text-xs text-container pl-1">{value}</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{data.CommandProfileName}</div>
      </div>
    );
  }, [data, value]);

  const itemTemplate = useCallback((option: SelectItem): JSX.Element => {
    return <div className="text-xs text-container">{option?.label ?? ''}</div>;
  }, []);

  if (!value && (!data?.CommandProfileName || data.IsCustomStream === true)) {
    return <div className="text-xs text-container  pl-1">StreamMaster</div>;
  }

  // if (data?.CommandProfileName === undefined) {
  //   return null;
  // }
  // Logger.debug('CommandProfileNameSelector', 'CommandProfileName', data?.CommandProfileName, data?.CommandProfileName ?? 'SystemDefault');
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
      value={data?.CommandProfileName ?? value}
      contentWidthSize="2"
    />
  );
};

CommandProfileNameSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(CommandProfileNameSelector);
