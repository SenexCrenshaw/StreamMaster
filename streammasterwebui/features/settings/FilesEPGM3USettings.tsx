import SettingsNameRegexDataSelector from '@components/settings/SettingsNameRegexDataSelector';
import { GetMessage } from '@lib/common/common';

import { Fieldset } from 'primereact/fieldset';
import React from 'react';
import { getCheckBoxLine } from './getCheckBoxLine';
import { getInputTextLine } from './getInputTextLine';
import { useSettingChangeHandler } from './useSettingChangeHandler';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { SMCard } from '@components/sm/SMCard';

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
    <SMCard
      darkBackGround={false}
      title="BACKUPS"
      header={<div className="justify-content-end align-items-center flex-row flex gap-1">{/* {header}                */}</div>}
    >
      <div className="sm-card-children">
        <div className="sm-card-children-content">
          {getCheckBoxLine({ currentSettingRequest, field: 'prettyEPG', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'cacheIcons', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'videoStreamAlwaysUseEPGLogo', onChange })}
          {getInputTextLine({ currentSettingRequest, field: 'dummyRegex', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'm3UIgnoreEmptyEPGID', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'm3UFieldGroupTitle', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'm3UUseChnoForId', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'm3UStationId', onChange })}
          {getCheckBoxLine({ currentSettingRequest, field: 'm3UUseCUIDForChannelID', onChange })}
          <Fieldset className="mt-4 pt-10" collapsed legend={GetMessage('nameregexSettings')} toggleable>
            <SettingsNameRegexDataSelector data={settingsQuery.data?.NameRegex} />
          </Fieldset>
        </div>
      </div>
    </SMCard>
  );
}
