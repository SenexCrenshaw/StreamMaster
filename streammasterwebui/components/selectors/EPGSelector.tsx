import { EpgColorDto, StationChannelName, useEpgFilesGetEpgColorsQuery, useSchedulesDirectGetStationChannelNamesQuery } from '@lib/iptvApi';
import { Dropdown } from 'primereact/dropdown';
import { classNames } from 'primereact/utils';
import React, { useEffect, useState } from 'react';

type EPGSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
};

const EPGSelector = ({ enableEditMode = true, value, disabled, editable, onChange }: EPGSelectorProperties) => {
  const [colors, setColors] = useState<EpgColorDto[]>([]);
  const [checkValue, setCheckValue] = useState<string | undefined>(undefined);
  const [stationChannelName, setStationChannelName] = useState<StationChannelName | undefined>(undefined);

  const query = useSchedulesDirectGetStationChannelNamesQuery();
  const colorsQuery = useEpgFilesGetEpgColorsQuery();

  useEffect(() => {
    if (checkValue === undefined && query.isSuccess && value) {
      setCheckValue(value);
      const entry = query.data?.find((x) => x.channel === value);
      if (entry && entry.id !== stationChannelName?.id) {
        setStationChannelName(entry);
      } else {
        setStationChannelName(undefined);
      }
    }
  }, [checkValue, query, stationChannelName?.id, value]);

  useEffect(() => {
    if (colors.length === 0 && colorsQuery.isSuccess && colorsQuery.data) {
      setColors(colorsQuery.data);
    }
  }, [colors.length, colorsQuery.data, colorsQuery.isSuccess]);

  const itemTemplate = (option: StationChannelName): JSX.Element => {
    if (!option) {
      return <div>{value}</div>;
    }

    let inputString = option?.displayName ?? '';
    const splitIndex = inputString.indexOf(']') + 1;
    const beforeCallSign = inputString.substring(0, splitIndex);
    const afterCallSign = inputString.substring(splitIndex).trim();

    const entry = colors.find((x) => x.stationId === option.channel);
    let color = '#FFFFFF';
    if (entry?.color) {
      color = entry.color;
    }

    if (beforeCallSign === '[' + afterCallSign + ']') {
      return (
        <div className="flex grid w-full align-items-center p-0 m-0">
          <div className="align-items-center pl-1 m-0 border-round ">
            <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
            <span className="text-md">{beforeCallSign}</span>
          </div>
        </div>
      );
    }

    return (
      <div className="flex grid w-full align-items-center p-0 m-0">
        <div className="align-items-center pl-1 m-0 border-round ">
          <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
          <span className="text-md">{beforeCallSign}</span>
          <div className="text-xs ml-5">{afterCallSign}</div>
        </div>
      </div>
    );
  };

  const className = classNames('BaseSelector align-contents-center p-0 m-0 max-w-full w-full epgSelector', {
    'p-disabled': disabled
  });

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0">{value ?? 'Dummy'}</div>;
  }

  const loading = !query.isSuccess || query.isFetching || query.isLoading;

  return (
    <div className="BaseSelector flex align-contents-center w-full min-w-full">
      <Dropdown
        className={className}
        disabled={loading}
        editable={editable}
        autoFocus
        filter
        filterBy="displayName"
        itemTemplate={itemTemplate}
        loading={loading}
        onChange={(e) => {
          setStationChannelName(e.value);
          if (onChange) {
            onChange(e.value.id);
          }
        }}
        onHide={() => {}}
        optionLabel="displayName"
        options={query.data}
        placeholder="placeholder"
        resetFilterOnHide
        showFilterClear
        value={stationChannelName}
        valueTemplate={itemTemplate}
        virtualScrollerOptions={{
          itemSize: 32,
          style: {
            minWidth: '400px',
            width: '400px',
            maxWidth: '50vw'
          }
        }}
      />
    </div>
  );
};

EPGSelector.displayName = 'EPGSelector';
export default React.memo(EPGSelector);
