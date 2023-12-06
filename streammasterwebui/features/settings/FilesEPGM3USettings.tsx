import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/common';
import { SettingDto, useSettingsGetSettingQuery } from '@lib/iptvApi';
import { useSelectCurrentSettingDto } from '@lib/redux/slices/selectedCurrentSettingDto';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';

export function FilesEPGM3USettings(): React.ReactElement {
  const settingsQuery = useSettingsGetSettingQuery();
  const { selectCurrentSettingDto, setSelectedCurrentSettingDto } = useSelectCurrentSettingDto('CurrentSettingDto');

  const onChange = (newValue: SettingDto) => {
    if (selectCurrentSettingDto === undefined || setSelectedCurrentSettingDto === undefined || newValue === null || newValue === undefined) return;
    setSelectedCurrentSettingDto(newValue);
  };

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('filesEPGM3U')}>
      {getCheckBoxLine({ field: 'cacheIcons', selectCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'videoStreamAlwaysUseEPGLogo', selectCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'dummyRegex', selectCurrentSettingDto, onChange })}
      <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
        <SettingsNameRegexDataSelector data={settingsQuery.data?.nameRegex} />
      </Fieldset>
      <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('m3uSettings')} toggleable>
        {getCheckBoxLine({ field: 'm3UIgnoreEmptyEPGID', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldCUID', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldChannelId', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldChannelNumber', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgName', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgChno', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgId', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgLogo', selectCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldGroupTitle', selectCurrentSettingDto, onChange })}
      </Fieldset>
    </Fieldset>
  );
}
