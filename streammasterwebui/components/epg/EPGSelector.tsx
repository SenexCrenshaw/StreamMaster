import AddButton from '@components/buttons/AddButton';
import StringEditor from '@components/inputs/StringEditor';
import { SMOverlay } from '@components/sm/SMOverlay';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSMContext } from '@lib/signalr/SMProvider';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import { EPGFileDto, StationChannelName } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import { Tooltip } from 'primereact/tooltip';
import React, { ReactNode, Suspense, lazy, useCallback, useEffect, useMemo, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

const SMScroller = lazy(() => import('@components/sm/SMScroller'));

type EPGResult = { epgNumber: number; stationId: string };

type EPGSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
  readonly simple?: boolean;
};

const EPGSelector = ({ enableEditMode = true, value, disabled, editable, onChange, simple }: EPGSelectorProperties) => {
  const { selectedItems } = useSelectedItems<EPGFileDto>('EPGSelector-EPGFiles');
  const [checkValue, setCheckValue] = useState<string | undefined>(undefined);
  const [stationChannelName, setStationChannelName] = useState<StationChannelName | undefined>(undefined);
  const [input, setInput] = useState<string | undefined>(undefined);
  const [newInput, setNewInput] = useState<string | undefined>(undefined);

  const { isSystemReady } = useSMContext();
  const query = useGetStationChannelNames();
  const epgQuery = useGetEPGFiles();
  const colorsQuery = useGetEPGColors();

  const epgFiles = useMemo(() => {
    const additionalOptions = [{ EPGNumber: -1, Name: 'SD' } as EPGFileDto];

    if (epgQuery.data) return [...additionalOptions, ...epgQuery.data];

    return epgQuery.data;
  }, [epgQuery]);

  useEffect(() => {
    if (value && !checkValue) {
      setInput(value);
    }
  }, [value, checkValue]);

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

    if (!query.data) {
      return undefined;
    }

    if (selectedItems && selectedItems.length > 0) {
      const epgNumbers = selectedItems.map((x) => x.EPGNumber);

      const r = query.data.filter((x) => {
        var test = extractEPGNumberAndStationId(x.Channel);
        return epgNumbers.includes(test.epgNumber);
      });

      return r;
    }
    return query.data;
  }, [extractEPGNumberAndStationId, query.data, selectedItems]);

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
  }, [checkValue, input, query.data, query.isError, stationChannelName]);

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

  const scrollerItemTemplate = useCallback(
    (option: EPGFileDto) => {
      const color = getColor(option.EPGNumber);
      return (
        <span className="sm-standard-text flex align-items-center justify-content-center">
          <span className="pi pi-circle-fill pr-2" style={{ color: color }} />
          <span className="text-xs"> {option.Name}</span>
        </span>
      );
    },
    [getColor]
  );

  const itemTemplate = useCallback(
    (option: StationChannelName): JSX.Element => {
      if (!option) {
        return <div className="text-xs text-container">{input}</div>;
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

      let tooltipClassName = `epgitem-${uuidv4()}  flex align-items-center`;

      if (beforeCallSign === '[' + afterCallSign + ']') {
        return (
          <>
            <Tooltip target={`${tooltipClassName}`} />
            <div className={`${tooltipClassName}`} data-pr-hidedelay={100} data-pr-position="left" data-pr-showdelay={500} data-pr-tooltip={epgName}>
              <span className="pi pi-circle-fill pr-2" style={{ color: color }} />
              <span className="text-xs text-container">{afterCallSign}</span>
            </div>
          </>
        );
      }

      return (
        <>
          <Tooltip target={`${tooltipClassName}`} />
          <div className={`${tooltipClassName} `} data-pr-hidedelay={100} data-pr-position="left" data-pr-showdelay={500} data-pr-tooltip={epgName}>
            <span className="pi pi-circle-fill pr-2" style={{ color: color }} />
            <span className="text-xs text-container width-200">
              <span className="text-xs text-container">{beforeCallSign}</span>
              <span className="sm-input-xs text-container">{afterCallSign}</span>
            </span>
          </div>
        </>
      );
    },
    [colorsQuery.data, epgQuery.data, extractEPGNumberAndStationId, input]
  );

  const valueTemplate = useCallback(
    (option2: StationChannelName | undefined): JSX.Element => {
      const stationChannelName = query.data?.find((x) => x.Channel === input);
      if (!stationChannelName) {
        const tooltipClassName = `epgitem-${uuidv4()}`;
        if (!input || input === '') {
          return <div className="text-xs text-container text-white-alpha-40">None</div>;
        }
        return (
          <>
            <Tooltip target={`.${tooltipClassName}`} />
            <span
              data-pr-hidedelay={100}
              data-pr-position="left"
              data-pr-showdelay={500}
              data-pr-tooltip={'No EPG'}
              className={`${tooltipClassName} text-xs text-container`}
            >
              {input}
            </span>
          </>
        );
      }

      let inputString = stationChannelName.DisplayName ?? '';
      const splitIndex = inputString.indexOf(']') + 1;
      const beforeCallSign = inputString.substring(0, splitIndex);
      // const afterCallSign = inputString.substring(splitIndex).trim();
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
            <i className="pl-1 pi pi-circle-fill" style={{ color: color }} />
            <span className="text-xs text-container">{beforeCallSign}</span>
          </div>
        </>
      );
    },
    [colorsQuery.data, epgQuery.data, extractEPGNumberAndStationId, input, query.data]
  );

  const handleOnChange = useCallback(
    (channel: string) => {
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
      onChange && onChange(channel);
    },
    [onChange, query.data, stationChannelName?.Channel]
  );

  const addDisabled = useMemo(() => {
    return checkValue === newInput;
  }, [checkValue, newInput]);

  const messageContent = useMemo(() => {
    return (
      <>
        <div className="flex flex-row w-12 sm-card border-radius-left border-radius-right">
          <Suspense>
            <div className="flex w-12 ml-1">
              <SMScroller
                data={options}
                dataKey="Channel"
                filter
                filterBy="DisplayName"
                itemSize={26}
                itemTemplate={itemTemplate}
                onChange={(e) => {
                  handleOnChange(e.Channel);
                }}
                scrollHeight={150}
                value={stationChannelName}
              />
            </div>
          </Suspense>
        </div>
        <div className="layout-padding-bottom-lg" />
        <div className="flex grid col-12 m-0 p-0 justify-content-between align-items-center">
          <div className="col-10 m-0 p-0 pl-2">
            <StringEditor
              darkBackGround
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
        <div className="layout-padding-bottom" />
      </>
    );
  }, [addDisabled, handleOnChange, input, itemTemplate, options, stationChannelName]);

  const headerValueTemplate = useMemo((): ReactNode => {
    if (selectedItems && selectedItems.length > 0) {
      const epgNames = selectedItems.slice(0, 2).map((x) => x.Name);
      const suffix = selectedItems.length > 2 ? ',...' : '';
      return (
        <div className="px-4 w-10rem flex align-content-center justify-content-center" style={{ minWidth: '10rem' }}>
          {epgNames.join(', ') + suffix}
        </div>
      );
    }
    return <div className="px-4 w-10rem" style={{ minWidth: '10rem' }} />;
  }, [selectedItems]);

  const headerTemplate = useMemo(() => {
    return (
      <div className="sm-input-dark">
        <SMOverlay widthSize="1" icon="pi-chevron-down" buttonTemplate={headerValueTemplate} buttonLabel="EPG" simple>
          <SMScroller
            data={epgFiles}
            dataKey="EPGNumber"
            filter={false}
            itemSize={26}
            itemTemplate={scrollerItemTemplate}
            scrollHeight={150}
            select
            selectedItemsKey="EPGSelector-EPGFiles"
            simple
          />
        </SMOverlay>
      </div>
    );
  }, [epgFiles, headerValueTemplate, scrollerItemTemplate]);

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0">{input ?? 'Dummy'}</div>;
  }

  const loading = query.isError || query.isFetching || query.isLoading || !query.data || isSystemReady !== true;

  if (loading) {
    return (
      <div className="flex align-content-center justify-content-center">
        <ProgressSpinner />
      </div>
    );
  }

  return (
    <SMOverlay header={headerTemplate} title="EPG" widthSize="2" icon="pi-chevron-down" buttonTemplate={valueTemplate(stationChannelName)} buttonLabel="EPG">
      <div>{messageContent}</div>
    </SMOverlay>
  );
};

EPGSelector.displayName = 'EPGSelector';
export default React.memo(EPGSelector);
