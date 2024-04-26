import AddButton from '@components/buttons/AddButton';
import StringEditorBodyTemplate from '@components/inputs/StringEditorBodyTemplate';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';

import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import { StationChannelName } from '@lib/smAPI/smapiTypes';
import { v4 as uuidv4 } from 'uuid';

import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import { Dropdown } from 'primereact/dropdown';
import { ProgressSpinner } from 'primereact/progressspinner';
import { Tooltip } from 'primereact/tooltip';
import { classNames } from 'primereact/utils';
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';

type EPGResult = { epgNumber: number; stationId: string };

type EPGSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
};

const EPGSelector = ({ enableEditMode = true, value, disabled, editable, onChange }: EPGSelectorProperties) => {
  const ref = useRef<Dropdown>(null);
  const [checkValue, setCheckValue] = useState<string | undefined>(undefined);
  const [stationChannelName, setStationChannelName] = useState<StationChannelName | undefined>(undefined);
  const [input, setInput] = useState<string | undefined>(undefined);
  const [newInput, setNewInput] = useState<string | undefined>(undefined);

  const query = useGetStationChannelNames();
  const epgQuery = useGetEPGFiles();
  const colorsQuery = useGetEPGColors();

  useEffect(() => {
    if (value && !input) {
      setInput(value);
    }
  }, [value, input]);

  useEffect(() => {
    if (checkValue === undefined && !query.isError && input) {
      setCheckValue(input);
      const entry = query.data?.find((x) => x.Channel === input);
      if (entry && entry.Channel !== stationChannelName?.Channel) {
        setStationChannelName(entry);
      } else {
        setStationChannelName(undefined);
      }
    }
  }, [checkValue, query.data, query.isError, input, stationChannelName]);

  function extractEPGNumberAndStationId(userTvgId: string): EPGResult {
    if (!userTvgId.trim()) {
      throw new Error('Input string cannot be null or whitespace.');
    }

    const regex = /^(\-?\d+)-(.*)/;
    const matches = userTvgId.match(regex);

    if (!matches || matches.length !== 3) {
      throw new Error('Input string is not in the expected format.');
    }

    const epgNumber = parseInt(matches[1], 10);
    if (isNaN(epgNumber)) {
      throw new Error('EPG number is not a valid number.');
    }

    const stationId = matches[2];

    return { epgNumber, stationId };
  }

  const itemTemplate = useCallback(
    (option: StationChannelName): JSX.Element => {
      if (!option) {
        return <div>{input}</div>;
      }

      let inputString = option?.DisplayName ?? '';
      const splitIndex = inputString.indexOf(']') + 1;
      const beforeCallSign = inputString.substring(0, splitIndex);
      const afterCallSign = inputString.substring(splitIndex).trim();
      let color = '#FFFFFF';

      if (colorsQuery?.data !== undefined) {
        const entry = colorsQuery.data.find((x) => x.StationId === option.Channel);
        if (entry?.Color) {
          color = entry.Color;
        }
      }

      let epgName = 'SD';
      const test = extractEPGNumberAndStationId(option.Channel);

      if (test.epgNumber > 0 && epgQuery.data !== undefined) {
        const entry = epgQuery.data.find((x) => x.EPGNumber === test.epgNumber);
        if (entry?.Name) {
          epgName = entry.Name;
        }
      }

      const tooltipClassName = `epgitem-${uuidv4()}`;

      // console.log(inputString);
      // console.log(beforeCallSign);
      // console.log(afterCallSign);
      if (beforeCallSign === '[' + afterCallSign + ']') {
        return (
          <>
            <Tooltip target={`.${tooltipClassName}`} />
            <div
              className={`${tooltipClassName} flex align-items-center border-white`}
              data-pr-hidedelay={100}
              data-pr-position="left"
              data-pr-showdelay={500}
              data-pr-tooltip={epgName}
            >
              <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
              <span className="text-xs">{afterCallSign}</span>
            </div>
          </>
        );
      }

      return (
        <>
          <Tooltip target={`.${tooltipClassName}`} />
          <div
            className={`${tooltipClassName} flex flex-column`}
            data-pr-hidedelay={100}
            data-pr-position="left"
            data-pr-showdelay={500}
            data-pr-tooltip={epgName}
          >
            <div className="flex flex-row">
              <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
              <span className="text-xs">{beforeCallSign}</span>
            </div>
            <div className="text-xs ml-5">{afterCallSign}</div>
          </div>
        </>
      );
    },
    [colorsQuery.data, epgQuery.data, input]
  );

  const valueTemplate = useCallback(
    (option: StationChannelName): JSX.Element => {
      if (!option) {
        return <div>{input}</div>;
      }

      let inputString = option?.DisplayName ?? '';
      const splitIndex = inputString.indexOf(']') + 1;
      // const beforeCallSign = inputString.substring(0, splitIndex);
      const afterCallSign = inputString.substring(splitIndex).trim();
      let color = '#FFFFFF';

      if (colorsQuery?.data !== undefined) {
        const entry = colorsQuery.data.find((x) => x.StationId === option.Channel);
        if (entry?.Color) {
          color = entry.Color;
        }
      }

      let epgName = 'SD';
      const test = extractEPGNumberAndStationId(option.Channel);

      if (test.epgNumber > 0 && epgQuery.data !== undefined) {
        const entry = epgQuery.data.find((x) => x.EPGNumber === test.epgNumber);
        if (entry?.Name) {
          epgName = entry.Name;
        }
      }

      const tooltipClassName = `epgitem-${uuidv4()}`;

      return (
        <>
          <Tooltip target={`.${tooltipClassName}`} />
          <div
            className={`${tooltipClassName} flex align-items-center border-white`}
            data-pr-hidedelay={100}
            data-pr-position="left"
            data-pr-showdelay={500}
            data-pr-tooltip={epgName}
          >
            <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
            <span className="text-xs">{afterCallSign}</span>
          </div>
        </>
      );
    },
    [colorsQuery.data, epgQuery.data, input]
  );

  const handleOnChange = (channel: string) => {
    if (!channel) {
      return;
    }

    const entry = query.data?.find((x) => x.Channel === channel);
    if (entry && entry.Channel !== stationChannelName?.Channel) {
      setStationChannelName(entry);
    } else {
      setStationChannelName(undefined);
    }

    setInput(channel);
    ref.current?.hide();
    onChange && onChange(channel);
  };

  const addDisabled = useMemo(() => {
    console.log(checkValue, newInput);
    return checkValue === newInput;
  }, [checkValue, newInput]);

  const panelTemplate = (option: any) => {
    return (
      <div className="flex grid col-12 m-0 p-0 justify-content-between align-items-center">
        <div className="col-11 m-0 p-0 pl-2">
          <StringEditorBodyTemplate
            disableDebounce={true}
            placeholder="Custom Id"
            value={input}
            onChange={(value) => {
              if (value) {
                setNewInput(value);
              }
            }}
            onSave={(value) => {
              if (value) {
                handleOnChange(value);
              }
            }}
          />
        </div>
        <div className="col-1 m-0 p-0">
          <AddButton
            disabled={addDisabled}
            tooltip="Add Custom Id"
            iconFilled
            onClick={(e) => {
              if (input) {
                handleOnChange(input);
              }
            }}
            style={{
              height: 'var(--input-height)',
              width: 'var(--input-height)'
            }}
          />
        </div>
      </div>
    );
  };

  const className = classNames('max-w-full w-full epgSelector', {
    'p-disabled': disabled
  });

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0">{input ?? 'Dummy'}</div>;
  }

  const loading = query.isError || query.isFetching || query.isLoading || !query.data;

  if (loading) {
    return <ProgressSpinner />;
  }

  return (
    <div className="sm-input flex align-contents-center w-full min-w-full h-full">
      <Dropdown
        className={className}
        disabled={loading}
        filterInputAutoFocus
        filter
        filterBy="DisplayName"
        itemTemplate={itemTemplate}
        loading={loading}
        onChange={(e) => {
          handleOnChange(e?.value?.Channel);
        }}
        onHide={() => {}}
        optionLabel="DisplayName"
        options={query.data}
        panelFooterTemplate={panelTemplate}
        placeholder="placeholder"
        ref={ref}
        resetFilterOnHide
        showFilterClear
        value={stationChannelName}
        valueTemplate={valueTemplate}
        virtualScrollerOptions={{
          itemSize: 24,
          style: {
            maxWidth: '50vw',
            minWidth: '400px',
            width: '400px'
          }
        }}
      />
    </div>
  );
};

EPGSelector.displayName = 'EPGSelector';
export default React.memo(EPGSelector);
