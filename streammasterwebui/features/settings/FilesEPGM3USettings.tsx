import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/common';
import { SettingDto, useSettingsGetSettingQuery } from '@lib/iptvApi';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';

type FilesEPGM3USettingsProps = {
  newData: SettingDto;
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>;
};

export function FilesEPGM3USettings({ newData, setNewData }: FilesEPGM3USettingsProps): React.ReactElement {
  const settingsQuery = useSettingsGetSettingQuery();

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('filesEPGM3U')}>
      {getCheckBoxLine({ field: 'cacheIcons', newData, setNewData })}
      {getCheckBoxLine({ field: 'videoStreamAlwaysUseEPGLogo', newData, setNewData })}
      {getInputTextLine({ field: 'dummyRegex', newData, setNewData })}
      <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
        <SettingsNameRegexDataSelector data={settingsQuery.data?.nameRegex} />
      </Fieldset>
      <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('m3uSettings')} toggleable>
        {getCheckBoxLine({ field: 'm3UIgnoreEmptyEPGID', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldCUID', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldChannelId', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldChannelNumber', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldTvgName', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldTvgChno', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldTvgId', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldTvgLogo', newData, setNewData })}
        {getCheckBoxLine({ field: 'm3UFieldGroupTitle', newData, setNewData })}
      </Fieldset>
    </Fieldset>
  );
}
