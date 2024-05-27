import { GetMessage } from '@lib/common/intl';
import React from 'react';
import { Fieldset } from 'primereact/fieldset';
import { SelectItem } from 'primereact/selectitem';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { getDropDownLine } from './components/getDropDownLine';
import { getInputNumberLine } from './components/getInputNumberLine';
import { getInputTextLine } from './components/getInputTextLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';
import { StreamingProxyTypes } from '@lib/smAPI/smapiTypes';
import { SMCard } from '@components/sm/SMCard';

export function StreamingSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

  const getHandlersOptions = (): SelectItem[] => {
    return Object.entries(StreamingProxyTypes)
      .splice(0, Object.keys(StreamingProxyTypes).length / 2)
      .map(([number, word]) => ({ label: word, value: number } as SelectItem));
  };

  if (!currentSettingRequest) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <SMCard darkBackGround={false} title="STREAMING" header={<div className="justify-content-end align-items-center flex-row flex gap-1"></div>}>
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          <div className="layout-padding-bottom" />
          <div className="settings-lines">
            {getDropDownLine({ currentSettingRequest, field: 'StreamingProxyType', onChange, options: getHandlersOptions() })}
            {getInputNumberLine({ currentSettingRequest, field: 'GlobalStreamLimit', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'ClientUserAgent', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'StreamingClientUserAgent', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'FFMpegOptions', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'ShowClientHostNames', onChange })}
          </div>
        </div>
        <div className="layout-padding-bottom" />
      </div>
    </SMCard>
  );
}
