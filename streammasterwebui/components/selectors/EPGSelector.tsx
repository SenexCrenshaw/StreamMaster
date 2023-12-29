import {
  EpgColorDto,
  StationChannelName,
  useSchedulesDirectGetPagedStationChannelNameSelectionsQuery,
  useSchedulesDirectGetStationChannelNamesSimpleQueryQuery
} from '@lib/iptvApi';

import { GetEpgColors } from '@lib/smAPI/EpgFiles/EpgFilesGetAPI';
import { GetStationChannelNameFromDisplayName } from '@lib/smAPI/SchedulesDirect/SchedulesDirectGetAPI';
import React, { useCallback, useEffect, useState } from 'react';
import BaseSelector, { type BaseSelectorProperties } from './BaseSelector';

type EPGSelectorProperties = BaseSelectorProperties<StationChannelName> & {
  enableEditMode?: boolean;
};

const EPGSelector: React.FC<Partial<EPGSelectorProperties>> = ({ enableEditMode = true, onChange, ...restProperties }) => {
  // const epgFilesGetEpgColorsQuery = useEpgFilesGetEpgColorsQuery();

  const [colors, setColors] = useState<EpgColorDto[]>([]);

  useEffect(() => {
    GetEpgColors()
      .then((x) => {
        if (x) {
          setColors(x);
        }
      })
      .catch((e) => {
        console.error(e);
      });
  }, []);

  const selectedTemplate = (option: any) => {
    const entry = colors.find((x) => x.stationId === restProperties.value);
    let color = '#FFFFFF';
    if (entry?.color) {
      // console.log('entry', entry);

      color = entry.color;
    }
    // const background = adjustBackgroundColorIfNeeded(color);

    return <div style={{ color: color }}>{option?.displayName}</div>;
  };

  const handleOnChange = useCallback(
    (event: string) => {
      if (event && onChange) {
        onChange(event);
      }
    },
    [onChange]
  );

  const itemTemplate = (option: any): JSX.Element => {
    let inputString = option?.displayName ?? '';
    const splitIndex = inputString.indexOf(']') + 1;
    const beforeCallSign = inputString.substring(0, splitIndex);
    const afterCallSign = inputString.substring(splitIndex).trim();
    // console.log('options', option);
    const entry = colors.find((x) => x.stationId === option.channel);
    let color = '#FFFFFF';
    if (entry?.color) {
      color = entry.color;
    }
    console.log('inputString', inputString);
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
        {/* <div className="col-5 align-items-center p-0 m-0 border-round"> */}
        <div className="align-items-center pl-1 m-0 border-round ">
          <i className="pi pi-circle-fill pr-2" style={{ color: color }} />
          <span className="text-md">{beforeCallSign}</span>
          <div className="text-xs ml-5">{afterCallSign}</div>
        </div>
        {/* </div> */}
        {/* <div className="col-fixed" style={{ width: '100px' }}>
          {afterCallSign}
        </div> */}
      </div>
    );
  };

  if (!enableEditMode) {
    return <div className="flex h-full justify-content-center align-items-center p-0 m-0">{restProperties.value ?? 'Dummy'}</div>;
  }

  return (
    <BaseSelector
      {...restProperties}
      editable
      itemSize={32}
      itemTemplate={itemTemplate}
      onChange={handleOnChange}
      optionLabel="channelName"
      optionValue="channel"
      queryFilter={useSchedulesDirectGetPagedStationChannelNameSelectionsQuery}
      queryHook={useSchedulesDirectGetStationChannelNamesSimpleQueryQuery}
      querySelectedItem={GetStationChannelNameFromDisplayName}
      selectName="EPG"
      selectedTemplate={selectedTemplate}
    />
  );
};

EPGSelector.displayName = 'EPGSelector';
export default React.memo(EPGSelector);
