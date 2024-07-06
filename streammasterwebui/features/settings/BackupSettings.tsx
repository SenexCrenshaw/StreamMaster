import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { getInputNumberLine } from './components/getInputNumberLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';
import { BaseSettings } from './BaseSettings';

export function BackupSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

  if (currentSettingRequest === null || currentSettingRequest === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="BACKUPS">
      <>
        {getCheckBoxLine({ currentSettingRequest, field: 'BackupEnabled', onChange })}
        {getInputNumberLine({ currentSettingRequest, field: 'BackupVersionsToKeep', onChange })}
        {getInputNumberLine({ currentSettingRequest, field: 'BackupInterval', onChange })}
      </>
    </BaseSettings>
  );
}
