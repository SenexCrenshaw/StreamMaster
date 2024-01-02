import { GetMessage } from '@lib/common/common';
import React from 'react';
// Import the getLine function
import { StreamingProxyTypes } from '@lib/common/streammaster_enums';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getDropDownLine } from './getDropDownLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';

export function StreamingSettings(): React.ReactElement {
  const { onChange, selectedCurrentSettingDto } = useSettingChangeHandler();

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

  if (selectedCurrentSettingDto === null || selectedCurrentSettingDto === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('streaming')} toggleable>
      {getDropDownLine({ field: 'streamingProxyType', options: getHandlersOptions(), selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'globalStreamLimit', selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'ringBufferSizeMB', min: 1, max: 256, selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'preloadPercentage', max: 999, selectedCurrentSettingDto, onChange })}

      {/* {getInputNumberLine('maxConnectRetry', 999)}
            {getInputNumberLine('maxConnectRetryTimeMS', 999)} */}
      {getInputTextLine({ field: 'clientUserAgent', selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'streamingClientUserAgent', selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'ffMpegOptions', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'showClientHostNames', selectedCurrentSettingDto, onChange })}
    </Fieldset>
  );
}
