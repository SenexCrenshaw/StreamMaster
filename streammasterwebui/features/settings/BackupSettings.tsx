import { GetMessage } from '@lib/common/common';
import React from 'react';
import { Fieldset } from 'primereact/fieldset';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputNumberLine } from './getInputNumberLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';
import { SMCard } from '@components/sm/SMCard';

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
    <SMCard
      darkBackGround={false}
      title="BACKUPS"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          {getCheckBoxLine({ currentSettingRequest, field: 'backupEnabled', onChange })}
          {getInputNumberLine({ currentSettingRequest, field: 'backupVersionsToKeep', onChange })}
          {getInputNumberLine({ currentSettingRequest, field: 'backupInterval', onChange })}
        </div>
      </div>
    </SMCard>
  );
}
