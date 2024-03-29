import AddButton from '@components/buttons/AddButton';
import StringEditorBodyTemplate from '@components/inputs/StringEditorBodyTemplate';

import { Dropdown } from 'primereact/dropdown';
import { ProgressSpinner } from 'primereact/progressspinner';
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
  const [input, setInput] = useState<string | undefined>(undefined);

  const query = useSchedulesDirectGetStationChannelNamesQuery();
  const colorsQuery = useEpgFilesGetEpgColorsQuery();

  useEffect(() => {
    if (value && !input) {
      setInput(value);
    }
  }, [value, input]);

  useEffect(() => {
    if (checkValue === undefined && query.isSuccess && input) {
      setCheckValue(input);
      const entry = query.data?.find((x) => x.channel === input);
      if (entry && entry.id !== stationChannelName?.id) {
        setStationChannelName(entry);
      } else {
        setStationChannelName(undefined);
      }
    }
  }, [checkValue, input, query, stationChannelName?.id]);

  useEffect(() => {
    if (colors.length === 0 && colorsQuery.isSuccess && colorsQuery.data) {
      setColors(colorsQuery.data);
    }
  }, [colors.length, colorsQuery.data, colorsQuery.isSuccess]);

  const itemTemplate = (option: StationChannelName): JSX.Element => {
    if (!option) {
      return <div>{input}</div>;
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

  const handleOnChange = (channel: string) => {
    if (!channel) {
      return;
    }

    const entry = query.data?.find((x) => x.channel === channel);
    if (entry && entry.channel !== stationChannelName?.channel) {
      setStationChannelName(entry);
    } else {
      setStationChannelName(undefined);
    }

    setInput(channel);
    onChange && onChange(channel);
  };

  const panelTemplate = (option: any) => {
    return (
      <div className="flex grid col-12 m-0 p-0 justify-content-between align-items-center">
        <div className="col-11 m-0 p-0 pl-2">
          <StringEditorBodyTemplate
            disableDebounce={true}
            placeholder="Custom URL"
            value={input}
            onChange={(value) => {
              if (value) {
                setInput(value);
              }
            }}
          />
        </div>
        <div className="col-1 m-0 p-0">
          <AddButton
            tooltip="Add Custom URL"
            iconFilled={false}
            onClick={(e) => {
              if (input) {
                handleOnChange(input);
              }
            }}
            style={{
              width: 'var(--input-height)',
              height: 'var(--input-height)'
            }}
          />
        </div>
      </div>
    );
  };

  const className = classNames('BaseSelector align-contents-center p-0 m-0 max-w-full w-full epgSelector', {
    'p-disabled': disabled
  });

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0">{input ?? 'Dummy'}</div>;
  }

  const loading = !query.isSuccess || query.isFetching || query.isLoading || !query.data;

  if (loading) {
    return <ProgressSpinner />;
  }

  return (
    <div className="BaseSelector flex align-contents-center w-full min-w-full">
      <Dropdown
        className={className}
        disabled={loading}
        filter
        filterBy="displayName"
        itemTemplate={itemTemplate}
        loading={loading}
        onChange={(e) => {
          handleOnChange(e?.value?.id);
        }}
        onHide={() => {}}
        optionLabel="displayName"
        options={query.data}
        panelFooterTemplate={panelTemplate}
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
