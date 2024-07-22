import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { GetInputNumberLine } from './components/GetInputNumberLine';

import { useSettingsContext } from '@lib/context/SettingsProvider';
import { BaseSettings } from './BaseSettings';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetInputTextLine } from './components/GetInputTextLine';

export function StreamingSettings(): React.ReactElement {
  const { currentSetting } = useSettingsContext();

  if (!currentSetting) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="STREAMING">
      <>
        {GetInputNumberLine({ field: 'GlobalStreamLimit' })}
        {GetInputTextLine({ field: 'StreamingClientUserAgent' })}
        {GetCheckBoxLine({ field: 'ShowClientHostNames' })}
      </>
    </BaseSettings>
  );
}
