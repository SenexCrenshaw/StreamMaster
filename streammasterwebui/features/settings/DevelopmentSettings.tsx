import { getTopToolOptions } from '@lib/common/common';
import { GetMessage } from '@lib/common/intl';
import { baseHostURL } from '@lib/settings';
import { Button } from 'primereact/button';
import { Fieldset } from 'primereact/fieldset';
import React from 'react';

export function DevelopmentSettings(): React.ReactElement {
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
