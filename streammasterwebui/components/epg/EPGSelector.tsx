import AddButton from '@components/buttons/AddButton';
import StringEditorBodyTemplate from '@components/inputs/StringEditorBodyTemplate';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';

import { VirtualScroller } from 'primereact/virtualscroller';

import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import { EPGFileDto, StationChannelName } from '@lib/smAPI/smapiTypes';
import { v4 as uuidv4 } from 'uuid';

import SideCar from '@components/SideCar';

import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import useGetIsSystemReady from '@lib/smAPI/Settings/useGetIsSystemReady';

import { Checkbox } from 'primereact/checkbox';
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

  const [selectedEPGFiles, setSelectedEPGFiles] = useState<EPGFileDto[]>([]);
  const [checkValue, setCheckValue] = useState<string | undefined>(undefined);
  const [stationChannelName, setStationChannelName] = useState<StationChannelName | undefined>(undefined);
  const [input, setInput] = useState<string | undefined>(undefined);
  const [newInput, setNewInput] = useState<string | undefined>(undefined);
  const getIsSystemReady = useGetIsSystemReady();

  const query = useGetStationChannelNames();
  const epgQuery = useGetEPGFiles();
  const colorsQuery = useGetEPGColors();

  const epgFiles = useMemo(() => {
    const additionalOptions = [{ EPGNumber: -1, Name: 'SD' } as EPGFileDto];

    // console.log('additionalOptions', additionalOptions);
    if (epgQuery.data) return [...additionalOptions, ...epgQuery.data];

    return additionalOptions;
  }, [epgQuery]);

  useEffect(() => {
    if (value && !checkValue) {
      setInput(value);
    }
  }, [value, checkValue]);

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

  const getColor = useCallback(
    (epgNumber: number) => {
      let color = '#FFFFFF';
      if (epgNumber < 0) {
        if (epgNumber === -99) {
          color = '#000000';
        }
      }

      if (epgNumber > 0 && colorsQuery?.data !== undefined) {
        const entry = colorsQuery.data.find((x) => x.EPGNumber === epgNumber);
        if (entry?.Color) {
          color = entry.Color;
        }
      }
      return color;
    },
    [colorsQuery]
  );

  const extractEPGNumberAndStationId = useCallback((userTvgId: string): EPGResult => {
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
  }, []);

  const scrollerItemTemplate = useCallback(
    (option: EPGFileDto) => {
      const color = getColor(option.EPGNumber);

      return (
        <div className="flex sm-input sm-scroller-item  align-items-center justify-content-start">
          <Checkbox
            className="sm-standard-text"
            inputId="ingredient1"
            name={option.Name}
            value={option}
            onChange={(e) => {
              console.log('checkbox', e);
              if (e.checked) {
                setSelectedEPGFiles([...selectedEPGFiles, option]);
              } else {
                setSelectedEPGFiles(selectedEPGFiles.filter((x) => x.EPGNumber !== option.EPGNumber));
              }
            }}
            checked={selectedEPGFiles.some((x) => x.EPGNumber === option.EPGNumber)}
          />
          <div className="ml-2 sm-standard-text flex align-items-center justify-content-center">
            <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
            {option.Name}
          </div>
        </div>
      );
    },
    [getColor, selectedEPGFiles]
  );

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
    [colorsQuery.data, epgQuery.data, extractEPGNumberAndStationId, input]
  );
  const valueTemplate = useCallback(
    (option2: StationChannelName): JSX.Element => {
      const stationChannelName = query.data?.find((x) => x.Channel === input);
      if (!stationChannelName) {
        return <div>{input}</div>;
      }

      let inputString = stationChannelName.DisplayName ?? '';
      const splitIndex = inputString.indexOf(']') + 1;
      // const beforeCallSign = inputString.substring(0, splitIndex);
      const afterCallSign = inputString.substring(splitIndex).trim();
      let color = '#FFFFFF';

      if (colorsQuery?.data !== undefined) {
        const entry = colorsQuery.data.find((x) => x.StationId === stationChannelName.Channel);
        if (entry?.Color) {
          color = entry.Color;
        }
      }

      let epgName = 'SD';
      const test = extractEPGNumberAndStationId(stationChannelName.Channel);

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
    [colorsQuery.data, epgQuery.data, extractEPGNumberAndStationId, input, query.data]
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
    return checkValue === newInput;
  }, [checkValue, newInput]);

  const panelTemplate = (option: any) => {
    return (
      <div className="flex grid col-12 m-0 p-0 justify-content-between align-items-center">
        <div className="col-1 m-0 p-0 pl-2">
          <SideCar anchorRef={ref}>
            <VirtualScroller items={epgFiles} itemTemplate={scrollerItemTemplate} itemSize={26} style={{ height: '30vh' }} />
          </SideCar>
        </div>
        <div className="col-10 m-0 p-0 pl-2">
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

  const options = useMemo(() => {
    // if (queryFilter.JSONFiltersString !== '[]') {
    //   const metaDataArray: SMDataTableFilterMetaData[] = JSON.parse(queryFilter.JSONFiltersString);
    //   if (metaDataArray) {
    //     const metaData = metaDataArray.filter((x) => x.fieldName === 'EPGId' && x.matchMode !== 'notContains');
    //     if (metaData.length > 0) {
    //       const toIgnore = metaData.flatMap((x) => x.value);
    //       return query.data?.filter((x) => toIgnore.some((prefix) => x.Channel.startsWith(prefix)));
    //     }
    //   }
    // }

    return query.data;
  }, [query.data]);

  const className = classNames('max-w-full w-full epgSelector', {
    'p-disabled': disabled
  });

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0">{input ?? 'Dummy'}</div>;
  }

  const loading = query.isError || query.isFetching || query.isLoading || !query.data || getIsSystemReady.data !== true;

  if (loading) {
    return (
      <div className="flex align-content-center justify-content-center">
        <ProgressSpinner />
      </div>
    );
  }

  return (
    <div className="sm-input flex align-contents-center w-full min-w-full h-full ">
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
        options={options}
        panelClassName="sm-epg-editor-panel"
        panelFooterTemplate={panelTemplate}
        placeholder="placeholder"
        ref={ref}
        resetFilterOnHide
        showFilterClear
        value={stationChannelName}
        valueTemplate={valueTemplate}
        virtualScrollerOptions={{
          itemSize: 26,
          style: { maxWidth: '50vw', minWidth: '400px', width: '400px' }
        }}
      />
    </div>
  );
};

EPGSelector.displayName = 'EPGSelector';
export default React.memo(EPGSelector);
