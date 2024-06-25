import { SMCard } from '@components/sm/SMCard';
import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { getInputNumberLine } from './components/getInputNumberLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';

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
      hasCloseButton
      darkBackGround={false}
      title="BACKUPS"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          <div className="layout-padding-bottom" />
          <div className="settings-lines ">
            {getCheckBoxLine({ currentSettingRequest, field: 'BackupEnabled', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'BackupVersionsToKeep', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'BackupInterval', onChange })}
          </div>
        </div>
        <div className="layout-padding-bottom" />
      </div>
    </SMCard>
  );
}
