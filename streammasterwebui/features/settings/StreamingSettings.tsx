import { GetMessage } from '@lib/common/common';
import React from 'react';

import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getDropDownLine } from './getDropDownLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';
import { StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { SMCard } from '@components/sm/SMCard';

export function StreamingSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

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

  if (currentSettingRequest === null || currentSettingRequest === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <SMCard
      darkBackGround={false}
      title="STREAMING"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          {getDropDownLine({ currentSettingRequest, field: 'streamingProxyType', onChange, options: getHandlersOptions() })}
          {getInputNumberLine({ currentSettingRequest, field: 'globalStreamLimit', onChange })}
          {getInputTextLine({ currentSettingRequest, field: 'clientUserAgent', onChange })}
          {getInputTextLine({ currentSettingRequest, field: 'streamingClientUserAgent', onChange })}
          {getInputTextLine({ currentSettingRequest, field: 'ffMpegOptions', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'showClientHostNames', onChange })}
        </div>
      </div>
    </SMCard>
    // <Fieldset className="mt-4 pt-10" legend={GetMessage('streaming')} toggleable>
    //   {getDropDownLine({ field: 'streamingProxyType', options: getHandlersOptions(), currentSettingRequest, onChange })}
    //   {getInputNumberLine({ field: 'globalStreamLimit', currentSettingRequest, onChange })}
    //   {getInputTextLine({ field: 'clientUserAgent', currentSettingRequest, onChange })}
    //   {getInputTextLine({ field: 'streamingClientUserAgent', currentSettingRequest, onChange })}
    //   {getInputTextLine({ field: 'ffMpegOptions', currentSettingRequest, onChange })}
    //   {getCheckBoxLine({ field: 'showClientHostNames', currentSettingRequest, onChange })}
    // </Fieldset>
  );
}
