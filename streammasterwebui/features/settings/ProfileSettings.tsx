import SettingsProfilesDataSelector from '@components/settings/SettingsProfilesDataSelector';
import { GetMessage } from '@lib/common/common';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';

export function ProfileSettings(): React.ReactElement {
  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('profiles')} toggleable>
      <SettingsProfilesDataSelector />
    </Fieldset>
  );
}
