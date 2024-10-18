import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import React from 'react';
import { BaseSettings } from './BaseSettings';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetInputNumberLine } from './components/GetInputNumberLine';
import { GetInputTextLine } from './components/GetInputTextLine';
import { GetPasswordLine } from './components/GetPasswordLine';

export function GeneralSettings(): React.ReactElement {
  const { currentSetting } = useSettingsContext();
  return (
    <BaseSettings title="GENERAL">
      <>
        {GetInputTextLine({ field: 'DeviceID' })}
        {GetCheckBoxLine({ field: 'CleanURLs' })}
        {GetInputTextLine({ field: 'FFMPegExecutable' })}
        {GetInputTextLine({ field: 'FFProbeExecutable' })}
        {GetCheckBoxLine({ field: 'EnableSSL' })}
        {currentSetting?.EnableSSL === true && (
          <>
            {GetInputTextLine({ field: 'SSLCertPath', warning: GetMessage('changesServiceRestart') })}
            {GetPasswordLine({
              field: 'SSLCertPassword',
              warning: GetMessage('changesServiceRestart')
            })}
          </>
        )}
        {/* {getCheckBoxLine({ currentSetting, field: 'EnablePrometheus', onChange })} */}
        {GetInputNumberLine({ field: 'MaxLogFiles' })}
        {GetInputNumberLine({ field: 'MaxLogFileSizeMB' })}
      </>
    </BaseSettings>
  );
}
