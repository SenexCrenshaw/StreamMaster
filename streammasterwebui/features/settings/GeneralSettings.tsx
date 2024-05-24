import { GetMessage } from '@lib/common/common';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';
import { SMCard } from '@components/sm/SMCard';

export function GeneralSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

  // if (currentSettingRequest === null || currentSettingRequest === undefined || 1 == 1) {
  //   return (
  //     <SMCard
  //       darkBackGround={false}
  //       title="GENERAL"
  //       header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
  //     >
  //       <div className="text-center">{GetMessage('loading')}</div>
  //     </SMCard>
  //   );
  // }

  return (
    <SMCard
      darkBackGround={false}
      title="GENERAL"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          {getInputTextLine({ currentSettingRequest, field: 'deviceID', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'cleanURLs', onChange })}
          {getInputTextLine({ currentSettingRequest, field: 'ffmPegExecutable', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'enableSSL', onChange })}
          {currentSettingRequest?.EnableSSL === true && (
            <>
              {getInputTextLine({ currentSettingRequest, field: 'sslCertPath', onChange, warning: GetMessage('changesServiceRestart') })}
              {getPasswordLine({
                currentSettingRequest,
                field: 'sslCertPassword',
                onChange,
                warning: GetMessage('changesServiceRestart')
              })}
            </>
          )}
          {getCheckBoxLine({ currentSettingRequest, field: 'enablePrometheus', onChange })}
          {getInputNumberLine({ currentSettingRequest, field: 'maxLogFiles', onChange })}
          {getInputNumberLine({ currentSettingRequest, field: 'maxLogFileSizeMB', onChange })}
        </div>
      </div>
    </SMCard>
  );
}
