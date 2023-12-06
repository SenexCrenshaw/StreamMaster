import { GetMessage } from '@lib/common/common';
import React from 'react';
// Import the getLine function
import { SettingDto } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import { Fieldset } from 'primereact/fieldset';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';

export function GeneralSettings(): React.ReactElement {
  const { selectCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');

  const onChange = (newValue: SettingDto) => {
    if (selectCurrentSettingDto === undefined || setSelectedCurrentSettingDto === undefined || newValue === null || newValue === undefined) return;
    setSelectedCurrentSettingDto(newValue);
  };

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('general')}>
      {getInputTextLine({ field: 'deviceID', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'cleanURLs', selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'ffmPegExecutable', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'enableSSL', selectCurrentSettingDto, onChange })}
      {selectCurrentSettingDto?.enableSSL === true && (
        <>
          {getInputTextLine({ field: 'sslCertPath', warning: GetMessage('changesServiceRestart'), selectCurrentSettingDto, onChange })}
          {getPasswordLine({ field: 'sslCertPassword', warning: GetMessage('changesServiceRestart'), selectCurrentSettingDto, onChange })}
        </>
      )}
      {getCheckBoxLine({ field: 'overWriteM3UChannels', selectCurrentSettingDto, onChange })}
      {/* {getCheckBoxLine('logPerformance')} */}
    </Fieldset>
  );
}
