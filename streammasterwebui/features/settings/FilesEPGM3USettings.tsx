import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/common';
import { useSettingsGetSettingQuery } from '@lib/iptvApi';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';

export function FilesEPGM3USettings(): React.ReactElement {
  const settingsQuery = useSettingsGetSettingQuery();
  const { onChange, selectedCurrentSettingDto } = useSettingChangeHandler();

  if (selectedCurrentSettingDto === null || selectedCurrentSettingDto === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('filesEPGM3U')} toggleable>
      {getCheckBoxLine({ field: 'cacheIcons', selectedCurrentSettingDto, onChange })}
      {getCheckBoxLine({ field: 'videoStreamAlwaysUseEPGLogo', selectedCurrentSettingDto, onChange })}
      {getInputTextLine({ field: 'dummyRegex', selectedCurrentSettingDto, onChange })}
      <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
        <SettingsNameRegexDataSelector data={settingsQuery.data?.nameRegex} />
      </Fieldset>
      <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('m3uSettings')} toggleable>
        {getCheckBoxLine({ field: 'm3UIgnoreEmptyEPGID', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldCUID', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldChannelId', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldChannelNumber', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgName', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgChno', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgId', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldTvgLogo', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UFieldGroupTitle', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UUseChnoForId', selectedCurrentSettingDto, onChange })}
        {getCheckBoxLine({ field: 'm3UStationId', selectedCurrentSettingDto, onChange })}
      </Fieldset>
    </Fieldset>
  );
}
