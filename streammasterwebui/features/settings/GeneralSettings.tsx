import { GetMessage } from '@lib/common/intl';
import React from 'react';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { getInputNumberLine } from './components/getInputNumberLine';
import { getInputTextLine } from './components/getInputTextLine';
import { getPasswordLine } from './components/getPasswordLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';
import { SMCard } from '@components/sm/SMCard';

export function GeneralSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();
  return (
    <SMCard
      darkBackGround={false}
      title="GENERAL"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          <div className="layout-padding-bottom" />
          <div className="settings-lines ">
            {getInputTextLine({ currentSettingRequest, field: 'DeviceID', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'CleanURLs', onChange })}
            {getInputTextLine({ currentSettingRequest, field: 'FFMPegExecutable', onChange })}
            {getCheckBoxLine({ currentSettingRequest, field: 'EnableSSL', onChange })}
            {currentSettingRequest?.EnableSSL === true && (
              <>
                {getInputTextLine({ currentSettingRequest, field: 'SSLCertPath', onChange, warning: GetMessage('changesServiceRestart') })}
                {getPasswordLine({
                  currentSettingRequest,
                  field: 'SSLCertPassword',
                  onChange,
                  warning: GetMessage('changesServiceRestart')
                })}
              </>
            )}
            {getCheckBoxLine({ currentSettingRequest, field: 'EnablePrometheus', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'MaxLogFiles', onChange })}
            {getInputNumberLine({ currentSettingRequest, field: 'MaxLogFileSizeMB', onChange })}
          </div>
          <div className="layout-padding-bottom" />
        </div>
      </div>
    </SMCard>
  );
}
