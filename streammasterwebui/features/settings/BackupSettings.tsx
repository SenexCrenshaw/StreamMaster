import { GetMessage } from '@lib/common/common';
import React from 'react';

import { Fieldset } from 'primereact/fieldset';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputNumberLine } from './getInputNumberLine';

import { useSettingChangeHandler } from './useSettingChangeHandler';

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
    <Fieldset className="mt-4 pt-10" legend={GetMessage('backups')} toggleable>
      {getCheckBoxLine({ field: 'backupEnabled', currentSettingRequest, onChange })}
      {getInputNumberLine({ field: 'backupVersionsToKeep', currentSettingRequest, onChange })}
      {getInputNumberLine({ field: 'backupInterval', currentSettingRequest, onChange })}
    </Fieldset>
  );
}
