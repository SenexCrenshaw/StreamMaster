import SettingsProfilesDataSelector from '@components/settings/SettingsProfilesDataSelector';
import { GetMessage } from '@lib/common/common';
import { useSettingsGetSettingQuery } from '@lib/iptvApi';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';

export function ProfileSettings(): React.ReactElement {
  const settingsQuery = useSettingsGetSettingQuery();

  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('profiles')} toggleable>
      <SettingsProfilesDataSelector data={settingsQuery.data?.ffmpegProfiles} />
    </Fieldset>
  );
}
