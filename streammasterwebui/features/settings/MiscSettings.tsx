import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/intl';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './components/getCheckBoxLine';
import { getInputTextLine } from './components/getInputTextLine';
import { useSettingChangeHandler } from './hooks/useSettingChangeHandler';
import { BaseSettings } from './BaseSettings';

export function MiscSettings(): React.ReactElement {
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
    <BaseSettings title="MISC">
      <>
        {getCheckBoxLine({ currentSettingRequest, field: 'PrettyEPG', onChange })}
        {getCheckBoxLine({ currentSettingRequest, field: 'AutoSetEPG', onChange })}
        {getCheckBoxLine({ currentSettingRequest, field: 'CacheIcons', onChange })}
        {getCheckBoxLine({ currentSettingRequest, field: 'VideoStreamAlwaysUseEPGLogo', onChange })}
        {getInputTextLine({ currentSettingRequest, field: 'DummyRegex', onChange })}
        {getCheckBoxLine({ currentSettingRequest, field: 'M3UIgnoreEmptyEPGID', onChange })}
        {/* {getCheckBoxLine({ currentSettingRequest, field: 'm3UFieldGroupTitle', onChange })} */}
        {/* {getCheckBoxLine({ currentSettingRequest, field: 'm3UUseChnoForId', onChange })} */}
        {/* {getCheckBoxLine({ currentSettingRequest, field: 'm3UStationId', onChange })} */}
        {/* {getCheckBoxLine({ currentSettingRequest, field: 'm3UUseCUIDForChannelID', onChange })} */}
        <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
          <SettingsNameRegexDataSelector data={settingsQuery.data?.NameRegex} />
        </Fieldset>
      </>
    </BaseSettings>
  );
}
