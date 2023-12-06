import { GetMessage, getTopToolOptions } from '@lib/common/common';
import { SettingDto } from '@lib/iptvApi';
import { baseHostURL } from '@lib/settings';
import { Button } from 'primereact/button';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';

type DevelopmentSettingsProps = {
  newData: SettingDto;
  setNewData: React.Dispatch<React.SetStateAction<SettingDto>>;
};

export function DevelopmentSettings({ newData, setNewData }: DevelopmentSettingsProps): React.ReactElement {
  return (
    <Fieldset className="mt-4 pt-10" legend={GetMessage('development')}>
      <Button
        icon="pi pi-bookmark-fill"
        label="Swagger"
        onClick={() => {
          const link = `${baseHostURL}/swagger`;
          window.open(link);
        }}
        tooltip="Swagger Link"
        tooltipOptions={getTopToolOptions}
      />
    </Fieldset>
  );
}
