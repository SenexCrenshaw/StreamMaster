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
  const { onChange, selectedCurrentSettingDto } = useSettingChangeHandler();

  if (selectedCurrentSettingDto === null || selectedCurrentSettingDto === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('general')} toggleable>
      {getInputTextLine({ field: 'deviceID', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'cleanURLs', selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'ffmPegExecutable', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'enableSSL', selectedCurrentSettingDto, onChange })}
      {selectedCurrentSettingDto?.enableSSL === true && (
        <>
          {getInputTextLine({ field: 'sslCertPath', warning: GetMessage('changesServiceRestart'), selectedCurrentSettingDto, onChange })}
          {getPasswordLine({ field: 'sslCertPassword', warning: GetMessage('changesServiceRestart'), selectedCurrentSettingDto, onChange })}
        </>
      )}
      {getCheckBoxLine({ field: 'enablePrometheus', selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'maxLogFiles', selectedCurrentSettingDto, onChange })}
      {getInputNumberLine({ field: 'maxLogFileSizeMB', selectedCurrentSettingDto, onChange })}
    </Fieldset>
  );
}
