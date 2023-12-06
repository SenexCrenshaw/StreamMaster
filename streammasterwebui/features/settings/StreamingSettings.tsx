import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import React from 'react';
// Import the getLine function
import { StreamingProxyTypes } from '@lib/common/streammaster_enums';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import { InputNumberLine } from './InputNumberLine';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getDropDownLine } from './getDropDownLine';
import { getInputTextLine } from './getInputTextLine';

type StreamingSettingsProps = {
  newData: SettingDto; // Adjust the type accordingly
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>; // Adjust the type accordingly
};

export function StreamingSettings({ newData, setNewData }: StreamingSettingsProps): React.ReactElement {
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

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('streaming')}>
      {getDropDownLine({ field: 'streamingProxyType', options: getHandlersOptions(), newData, setNewData })}
      {InputNumberLine({ field: 'globalStreamLimit', newData, setNewData })}
      {InputNumberLine({ field: 'ringBufferSizeMB', newData, setNewData })}
      {InputNumberLine({ field: 'preloadPercentage', max: 999, newData, setNewData })}

      {/* {getInputNumberLine('maxConnectRetry', 999)}
            {getInputNumberLine('maxConnectRetryTimeMS', 999)} */}
      {getInputTextLine({ field: 'clientUserAgent', newData, setNewData })}
      {getInputTextLine({ field: 'streamingClientUserAgent', newData, setNewData })}
      {getInputTextLine({ field: 'ffMpegOptions', newData, setNewData })}
      {getCheckBoxLine({ field: 'showClientHostNames', newData, setNewData })}
    </Fieldset>
  );
}
