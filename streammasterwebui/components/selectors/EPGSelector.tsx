import {
  EpgColorDto,
  StationChannelName,
  useSchedulesDirectGetPagedStationChannelNameSelectionsQuery,
  useSchedulesDirectGetStationChannelNamesSimpleQueryQuery
} from '@lib/iptvApi';
import chroma from 'chroma-js';

import { getColor } from '@lib/common/colors';
import { GetEpgColors } from '@lib/smAPI/EpgFiles/EpgFilesGetAPI';
import { GetStationChannelNameFromDisplayName } from '@lib/smAPI/SchedulesDirect/SchedulesDirectGetAPI';
import React, { useCallback, useEffect, useState } from 'react';
import BaseSelector, { type BaseSelectorProperties } from './BaseSelector';

type EPGSelectorProperties = BaseSelectorProperties<StationChannelName> & {
  enableEditMode?: boolean;
  readonly epgFileId: number;
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
        console.log(e);
      });
  }, []);

  function adjustBackgroundColorIfNeeded(foregroundColor: string, threshold: number = 4.5): string {
    const contrastRatio = chroma.contrast(foregroundColor, '2F394A');

    if (contrastRatio < threshold) {
      // Calculate the new background color with adjusted lightness (brightness)
      //const newBackgroundColor = chroma('2F394A').luminance() > 0.5 ? chroma('2F394A').darken(20) : chroma('2F394A').brighten(20);

      return '#5c708c'; //newBackgroundColor.hex();
    }

    return '#2F394A';
  }

  const selectedTemplate = (option: any) => {
    const entry = colors.find((x) => x.stationId === restProperties.value);
    let color = '#FFFFFF';
    if (entry?.color) {
      // console.log('entry', entry);
      color = '#' + entry.color;
    }
    const background = adjustBackgroundColorIfNeeded(color);

    console.log('option', option.displayName, getColor(option?.displayName));
    return <div style={{ color: color, backgroundColor: background }}>{option?.displayName} shit</div>;
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
    const afterCallSign = inputString.substring(splitIndex);

    const entry = colors.find((x) => x.stationId === restProperties.value);
    let color = '#FFFFFF';
    if (entry?.color) {
      color = '#' + entry.color;
    }
    const background = adjustBackgroundColorIfNeeded(color);

    return (
      // <div>
      //   <span style={{ color: getColor(beforeCallSign) }}>{beforeCallSign}</span>
      //   {afterCallSign}
      // </div>
      <div className="flex grid w-full align-items-center p-0 m-0">
        <div className="col-3 align-items-center p-0 m-0 border-round">
          <div className="align-items-center pl-1 m-0 border-round " style={{ color: color, backgroundColor: background }}>
            {beforeCallSign}
          </div>
        </div>
        <div className="col-fixed" style={{ width: '100px' }}>
          {afterCallSign}
        </div>
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
