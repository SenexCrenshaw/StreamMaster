import ProfilesDataSelector from '@components/Profiles/ProfilesDataSelector';
import { GetMessage } from '@lib/common/intl';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';

export function ProfileSettings(): React.ReactElement {
  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('profiles')} toggleable>
      <ProfilesDataSelector />
    </Fieldset>
  );
}
