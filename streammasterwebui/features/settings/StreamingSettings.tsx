import { GetMessage } from '@lib/common/common';
import React from 'react';
// Import the getLine function
import { StreamingProxyTypes } from '@lib/common/streammaster_enums';
import { SettingDto } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getDropDownLine } from './getDropDownLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';

export function StreamingSettings(): React.ReactElement {
  const { selectCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');

  const getHandlersOptions = (): SelectItem[] => {
    const test = Object.entries(StreamingProxyTypes)
      .splice(0, Object.keys(StreamingProxyTypes).length / 2)
      .map(
        ([number, word]) =>
          ({
            label: word,
            value: number
          } as SelectItem)
      );

    return test;
  };

  const onChange = (newValue: SettingDto) => {
    if (selectCurrentSettingDto === undefined || setSelectedCurrentSettingDto === undefined || newValue === null || newValue === undefined) return;
    setSelectedCurrentSettingDto(newValue);
  };

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('streaming')}>
      {getDropDownLine({ field: 'streamingProxyType', options: getHandlersOptions(), selectCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'globalStreamLimit', selectCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'ringBufferSizeMB', selectCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'preloadPercentage', max: 999, selectCurrentSettingDto, onChange })}

      {/* {getInputNumberLine('maxConnectRetry', 999)}
            {getInputNumberLine('maxConnectRetryTimeMS', 999)} */}
      {getInputTextLine({ field: 'clientUserAgent', selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'streamingClientUserAgent', selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'ffMpegOptions', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'showClientHostNames', selectCurrentSettingDto, onChange })}
    </Fieldset>
  );
}
