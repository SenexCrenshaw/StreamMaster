import AddButton from '@components/buttons/AddButton';
import StringEditor from '@components/inputs/StringEditor';
import SMButton from '@components/sm/SMButton';
import SMDropDown, { SMDropDownRef } from '@components/sm/SMDropDown';
import { useSMContext } from '@lib/context/SMProvider';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import useGetEPGColors from '@lib/smAPI/EPG/useGetEPGColors';
import useGetEPGFiles from '@lib/smAPI/EPGFiles/useGetEPGFiles';
import useGetStationChannelNames from '@lib/smAPI/SchedulesDirect/useGetStationChannelNames';
import { EPGFileDto, SMChannelDto, StationChannelName } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import { Tooltip } from 'primereact/tooltip';
import React, { ReactNode, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

type EPGResult = { epgNumber: number; stationId: string };

type EPGSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly smChannel?: SMChannelDto;
  readonly onChange?: (value: string) => void;
  readonly buttonDarkBackground?: boolean;
  readonly value?: string | undefined;
};

const EPGSelector = ({ buttonDarkBackground = false, value, enableEditMode = true, label, smChannel, isLoading, onChange }: EPGSelectorProperties) => {
  const { selectedItems } = useSelectedItems<EPGFileDto>('EPGSelector-EPGFiles');
  const [checkValue, setCheckValue] = useState<string | undefined>(undefined);
  const [stationChannelName, setStationChannelName] = useState<StationChannelName | undefined>(undefined);
  const [input, setInput] = useState<string | undefined>(undefined);
  const [newInput, setNewInput] = useState<string | undefined>(undefined);
  const dropDownRef = useRef<SMDropDownRef>(null);
  const [originalValue, setOriginalValue] = useState<string | undefined | null>(undefined);
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
    if (smChannel && !checkValue) {
      setInput(smChannel.EPGId);
      return;
    }
  }, [smChannel, checkValue]);

  useEffect(() => {
    if (!smChannel || !input) {
      return;
    }

    if (smChannel.EPGId !== input) {
      setInput(smChannel.EPGId);
    }
  }, [smChannel, input]);

  useEffect(() => {
    if (originalValue === undefined) {
      if (value !== undefined) {
        setOriginalValue(value);
        setInput(value);
      }
    } else if (value !== originalValue) {
      setOriginalValue(value);
      setInput(value);
    }
  }, [originalValue, value]);

  const extractEPGNumberAndStationId = useCallback((userTvgId: string): EPGResult => {
    if (!userTvgId.trim()) {
      return { epgNumber: -1, stationId: '-1' };
    }

    const regex = /^(-?\d+)-(.*)/;
    const matches = userTvgId.match(regex);

    if (!matches || matches.length !== 3) {
      return { epgNumber: -1, stationId: '-1' };
    }

    const epgNumber = parseInt(matches[1], 10);
    if (isNaN(epgNumber)) {
      return { epgNumber: -1, stationId: '-1' };
    }

    const stationId = matches[2];

    return { epgNumber, stationId };
  }, []);

  const options = useMemo(() => {
    if (!query.data) {
      return undefined;
    }

    // const test = query.data.filter((x) => {
    //   return x.Channel.startsWith('-1');
    // });

    // Logger.debug('EPGSelector', { test });

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
      if (option === undefined) {
        return null;
      }
      const color = getColor(option.EPGNumber);
      return (
        <span className="sm-standard-text flex align-items-center justify-content-start">
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
            <Tooltip target={`.${tooltipClassName}`} />
            <div className={`${tooltipClassName}`} data-pr-hidedelay={100} data-pr-position="left" data-pr-showdelay={500} data-pr-tooltip={epgName}>
              <span className="pi pi-circle-fill pr-2" style={{ color: color }} />
              <span className="text-xs text-container">{afterCallSign}</span>
            </div>
          </>
        );
      }

      return (
        <>
          <Tooltip target={`.${tooltipClassName}`} />
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

  const buttonTemplate = useCallback(
    (option2: StationChannelName | undefined): JSX.Element => {
      const tooltipClassName = `epgitem-${uuidv4()}`;

      if (!input || input === '') {
        return (
          <div className="sm-epg-selector">
            <i
              className={`${tooltipClassName} pl-1 pi pi-circle-fill`}
              style={{ color: '#000000' }}
              data-pr-hidedelay={100}
              data-pr-position="left"
              data-pr-showdelay={500}
              data-pr-tooltip="No EPG"
            />
            <div className="text-xs text-container text-white-alpha-40 pl-1">Dummy</div>
          </div>
        );
      }

      if (input.startsWith('-3-')) {
        const epgName = input.substring(3);
        let color = '#124482';

        return (
          <div className="sm-epg-selector">
            <Tooltip target={`.${tooltipClassName}`} />
            <i
              className={`${tooltipClassName} pl-1 pi pi-circle-fill`}
              style={{ color: color }}
              data-pr-hidedelay={100}
              data-pr-position="left"
              data-pr-showdelay={500}
              data-pr-tooltip={epgName}
            />
            <div className="text-xs text-container text-white-alpha-40 pl-1">{epgName}</div>
          </div>
        );
      }

      const stationChannelName = query.data?.find((x) => x.Channel === input);
      if (!stationChannelName) {
        return (
          <div className="sm-epg-selector">
            <Tooltip target={`.${tooltipClassName}`} />
            <span
              data-pr-hidedelay={100}
              data-pr-position="left"
              data-pr-showdelay={500}
              data-pr-tooltip={'No EPG'}
              className={`${tooltipClassName} text-xs text-container`}
            >
              <div className="text-xs text-container text-white-alpha-40 pl-1">{input}</div>
            </span>
          </div>
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

      return (
        <div className="sm-epg-selector">
          <Tooltip target={`.${tooltipClassName}`} />
          <i
            className={`${tooltipClassName} pl-1 pi pi-circle-fill`}
            style={{ color: color }}
            data-pr-hidedelay={100}
            data-pr-position="left"
            data-pr-showdelay={500}
            data-pr-tooltip={epgName}
          />
          <div className="text-container pl-1">{beforeCallSign}</div>
        </div>
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

      onChange && onChange(channel);
    },
    [onChange, query.data, stationChannelName?.Channel]
  );

  const addDisabled = useMemo(() => {
    return checkValue === newInput;
  }, [checkValue, newInput]);

  const headerValueTemplate = useMemo((): ReactNode => {
    if (selectedItems && selectedItems.length > 0) {
      const epgNames = selectedItems.slice(0, 2).map((x) => x.Name);
      const suffix = selectedItems.length > 2 ? ',...' : '';
      return <div className="px-4 w-10rem flex align-content-center justify-content-center min-w-10rem dark-background">{epgNames.join(', ') + suffix}</div>;
    }
    return <div className="pl-1 dark-background">All EPGs</div>;
  }, [selectedItems]);

  const headerTemplate = useMemo(() => {
    return (
      <SMDropDown
        buttonDarkBackground
        buttonTemplate={headerValueTemplate}
        contentWidthSize="2"
        data={epgFiles}
        dataKey="EPGNumber"
        info=""
        itemTemplate={scrollerItemTemplate}
        scrollHeight="20vh"
        select
        selectedItemsKey="EPGSelector-EPGFiles"
        simple
        title="EPG"
      />
    );
  }, [epgFiles, headerValueTemplate, scrollerItemTemplate]);
  const footerTemplate = useMemo(() => {
    return (
      <>
        <div className="flex grid sm-w-12 m-0 p-0 justify-content-between align-items-center">
          <div className="sm-w-6">{headerTemplate}</div>
          <div className="pl-2 flex flex-row sm-w-6 gap-1">
            <StringEditor
              darkBackGround
              disableDebounce
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
            <div className="flex flex-row gap-1">
              <SMButton
                icon="pi-cog"
                buttonClassName="icon-orange"
                tooltip="Set Dummy"
                iconFilled
                onClick={(e) => {
                  handleOnChange('-2-DUMMY');
                  dropDownRef.current?.hide();
                }}
              />
              <AddButton
                buttonDisabled={addDisabled}
                tooltip="Add Custom Id"
                iconFilled
                onClick={(e) => {
                  if (input) {
                    handleOnChange(input);
                  }
                }}
              />
            </div>
          </div>
        </div>
      </>
    );
  }, [addDisabled, handleOnChange, headerTemplate, input]);

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

  if (smChannel?.IsSystem === true) {
    const epgName = smChannel.Name;
    let color = '#124482';

    let tooltipClassName = `epgitem-${uuidv4()}  flex align-items-center`;

    return (
      <div className="sm-epg-selector">
        <Tooltip target={`.${tooltipClassName}`} />
        <i
          className={`${tooltipClassName} pl-1 pi pi-circle-fill`}
          style={{ color: color }}
          data-pr-hidedelay={100}
          data-pr-position="left"
          data-pr-showdelay={500}
          data-pr-tooltip={epgName}
        />
        <div className="text-xs text-container pl-1">{epgName}</div>
      </div>
    );
  }

  return (
    <SMDropDown
      buttonDisabled={smChannel === undefined}
      buttonDarkBackground={buttonDarkBackground}
      buttonIsLoading={loading || isLoading}
      buttonLabel="EPG"
      buttonTemplate={buttonTemplate(stationChannelName)}
      contentWidthSize="3"
      data={options}
      dataKey="Channel"
      filter
      filterBy="DisplayName"
      header={footerTemplate}
      info=""
      itemTemplate={itemTemplate}
      label={label}
      onChange={(e) => {
        handleOnChange(e.Channel);
      }}
      ref={dropDownRef}
      title="EPG"
      value={stationChannelName}
    />
  );
};

EPGSelector.displayName = 'EPGSelector';
export default React.memo(EPGSelector);
