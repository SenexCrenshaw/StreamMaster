import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/intl';
import { useSettingsContext } from '@lib/context/SettingsProvider';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { BaseSettings } from './BaseSettings';
import { GetCheckBoxLine } from './components/GetCheckBoxLine';
import { GetInputTextLine } from './components/GetInputTextLine';

export function MiscSettings(): React.ReactElement {
  const settingsQuery = useGetSettings();
  const { currentSetting } = useSettingsContext();
  if (currentSetting === null || currentSetting === undefined) {
    return (
      <Fieldset className="mt-4 pt-10" legend={GetMessage('SD')}>
        <div className="text-center">{GetMessage('loading')}</div>
      </Fieldset>
    );
  }

  return (
    <BaseSettings title="MISC">
      <>
        {GetCheckBoxLine({ field: 'PrettyEPG' })}
        {/* {getCheckBoxLine({  field: 'AutoSetEPG' })} */}
        {GetCheckBoxLine({ field: 'CacheIcons' })}
        {/* {getCheckBoxLine({  field: 'VideoStreamAlwaysUseEPGLogo' })} */}
        {GetInputTextLine({ field: 'DummyRegex' })}
        {GetCheckBoxLine({ field: 'M3UIgnoreEmptyEPGID' })}
        {/* {getCheckBoxLine({  field: 'm3UFieldGroupTitle' })} */}
        {/* {getCheckBoxLine({  field: 'm3UUseChnoForId' })} */}
        {/* {getCheckBoxLine({  field: 'm3UStationId' })} */}
        {/* {getCheckBoxLine({  field: 'm3UUseCUIDForChannelID' })} */}
        <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
          <SettingsNameRegexDataSelector data={settingsQuery.data?.NameRegex} />
        </Fieldset>
      </>
    </BaseSettings>
  );
}
