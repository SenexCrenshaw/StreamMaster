
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { BaseSettings } from './BaseSettings';
import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetInputNumberLine } from './components/GetInputNumberLine';

export function BackupSettings(): React.ReactElement {
  const { currentSetting } = useSettingsContext();
  if (currentSetting === null || currentSetting === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="BACKUPS">
      <>
        {GetCheckBoxLine({ field: 'BackupEnabled' })}
        {GetInputNumberLine({ field: 'BackupVersionsToKeep' })}
        {GetInputNumberLine({ field: 'BackupInterval' })}
      </>
    </BaseSettings>
  );
}
