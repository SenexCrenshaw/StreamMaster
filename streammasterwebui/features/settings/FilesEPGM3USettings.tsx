import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/common';

import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';

export function FilesEPGM3USettings(): React.ReactElement {
  const settingsQuery = useGetSettings();
  const { onChange, currentSettingRequest } = useSettingChangeHandler();

  if (currentSettingRequest === null || currentSettingRequest === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('filesEPGM3U')} toggleable>
      {getCheckBoxLine({ field: 'prettyEPG', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'cacheIcons', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'videoStreamAlwaysUseEPGLogo', currentSettingRequest, onChange })}
      {getInputTextLine({ field: 'dummyRegex', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'm3UIgnoreEmptyEPGID', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'm3UFieldGroupTitle', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'm3UUseChnoForId', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'm3UStationId', currentSettingRequest, onChange })}
      {getCheckBoxLine({ field: 'm3UUseCUIDForChannelID', currentSettingRequest, onChange })}
      <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
        <SettingsNameRegexDataSelector data={settingsQuery.data?.NameRegex} />
      </Fieldset>
    </Fieldset>
  );
}
