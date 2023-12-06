import { GetMessage } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';
import { getPasswordLine } from './getPasswordLine';

type SDSettingsProps = {
  newData: SettingDto;
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>;
};

export function SDSettings({ newData, setNewData }: SDSettingsProps): React.ReactElement {
  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
      {getCheckBoxLine({ field: 'sdSettings.sdEnabled', newData, setNewData })}
      {getInputTextLine({ field: 'sdSettings.sdUserName', newData, setNewData })}
      {getPasswordLine({ field: 'sdSettings.sdPassword', newData, setNewData })}
      {getCheckBoxLine({ field: 'sdSettings.seriesPosterArt', newData, setNewData })}
    </Fieldset>
  );
}
