import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import React from 'react';
// Import the getLine function
import { Fieldset } from 'primereact/fieldset';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';

type GeneralSettingsProps = {
  newData: SettingDto; // Adjust the type accordingly
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>; // Adjust the type accordingly
};

export function GeneralSettings({ newData, setNewData }: GeneralSettingsProps): React.ReactElement {
  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('general')}>
      {getInputTextLine({ field: 'deviceID', newData, setNewData })}
      {getCheckBoxLine({ field: 'cleanURLs', newData, setNewData })}
      {getInputTextLine({ field: 'ffmPegExecutable', newData, setNewData })}
      {getCheckBoxLine({ field: 'enableSSL', newData, setNewData })}
      {newData.enableSSL === true && (
        <>
          {getInputTextLine({ field: 'sslCertPath', warning: GetMessage('changesServiceRestart'), newData, setNewData })}
          {getPasswordLine({ field: 'sslCertPassword', warning: GetMessage('changesServiceRestart'), newData, setNewData })}
        </>
      )}
      {getCheckBoxLine({ field: 'overWriteM3UChannels', newData, setNewData })}
      {/* {getCheckBoxLine('logPerformance')} */}
    </Fieldset>
  );
}
