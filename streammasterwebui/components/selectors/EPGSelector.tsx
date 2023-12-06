import {
  StationChannelName,
  useSchedulesDirectGetPagedStationChannelNameSelectionsQuery,
  useSchedulesDirectGetStationChannelNamesSimpleQueryQuery
} from '@lib/iptvApi';

import { getColor } from '@lib/common/colors';
import { GetStationChannelNameFromDisplayName } from '@lib/smAPI/SchedulesDirect/SchedulesDirectGetAPI';
import React, { useCallback } from 'react';
import BaseSelector, { type BaseSelectorProperties } from './BaseSelector';

type EPGSelectorProperties = BaseSelectorProperties<StationChannelName> & {
  enableEditMode?: boolean;
};

const EPGSelector: React.FC<Partial<EPGSelectorProperties>> = ({ enableEditMode = true, onChange, ...restProperties }) => {
  const selectedTemplate = (option: StationChannelName) => <div>{option?.displayName}</div>;

  const handleOnChange = useCallback(
    (event: string) => {
      if (event && onChange) {
        onChange(event);
      }
    },
    [onChange]
  );

  const itemTemplate = (option: StationChannelName): JSX.Element => {
    let inputString = option?.displayName ?? '';
    const splitIndex = inputString.indexOf(']') + 1;
    const beforeCallSign = inputString.substring(0, splitIndex);
    const afterCallSign = inputString.substring(splitIndex);

    return (
      // <div>
      //   <span style={{ color: getColor(beforeCallSign) }}>{beforeCallSign}</span>
      //   {afterCallSign}
      // </div>
      <div className="flex grid w-full">
        <div className="col-3">
          <div style={{ color: getColor(beforeCallSign) }}>{beforeCallSign}</div>
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
