import { GetMessage } from '@lib/common/common';
import React from 'react';
// Import the getLine function
import { Fieldset } from 'primereact/fieldset';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputNumberLine } from './getInputNumberLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';

export function GeneralSettings(): React.ReactElement {
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

  if (currentSettingRequest === null || currentSettingRequest === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('general')} toggleable>
      {getInputTextLine({ field: 'deviceID', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'cleanURLs', currentSettingRequest, onChange })}
      {getInputTextLine({ field: 'ffmPegExecutable', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'enableSSL', currentSettingRequest, onChange })}
      {currentSettingRequest?.EnableSSL === true && (
        <>
          {getInputTextLine({ field: 'sslCertPath', warning: GetMessage('changesServiceRestart'), currentSettingRequest, onChange })}
          {getPasswordLine({ field: 'sslCertPassword', warning: GetMessage('changesServiceRestart'), currentSettingRequest, onChange })}
        </>
      )}
      {getCheckBoxLine({ field: 'enablePrometheus', currentSettingRequest, onChange })}
      {getInputNumberLine({ field: 'maxLogFiles', currentSettingRequest, onChange })}
      {getInputNumberLine({ field: 'maxLogFileSizeMB', currentSettingRequest, onChange })}
    </Fieldset>
  );
}
