import { Button } from 'primereact/button';
import { InputText } from 'primereact/inputtext';
import React from 'react';

import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';
import { UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

const SettingsNameRegexAddDialog = (props: SettingsNameRegexAddDialogProperties) => {
  const [regex, setRegex] = React.useState<string | undefined>('');

  const ReturnToParent = React.useCallback(() => {
    setRegex('');
    props.onClose?.();
  }, [props]);

  const onSave = React.useCallback(() => {
    if (!regex || regex === '') {
      ReturnToParent();

      return;
    }

    const tosend = {} as UpdateSettingRequest;

    tosend.parameters.NameRegex = [regex, ...props.values];

    UpdateSetting(tosend)
      .then(() => {})
      .catch((error) => {
        Logger.error(`Add Regex Error: ${error.message}`);
      });
  }, [ReturnToParent, props.values, regex]);

  return (
    <SMPopUp header="Add Regex" icon="pi pi-plus" tooltip="Edit Group">
      <div className="m-0 p-0 border-1 border-round surface-border">
        <div className="m-3">
          <InputText autoFocus className="withpadding p-inputtext-sm w-full" onChange={(e) => setRegex(e.target.value)} placeholder="Regex" value={regex} />

          <div className="card flex mt-3 flex-wrap gap-1 justify-content-center">
            <Button icon="pi pi-times" label="Cancel" onClick={() => ReturnToParent()} rounded severity="warning" />
            <Button disabled={!regex || regex === ''} icon="pi pi-check" label="Ok" onClick={onSave} rounded severity="success" />
          </div>
        </div>
      </div>
    </SMPopUp>
  );
};

SettingsNameRegexAddDialog.displayName = 'SettingsNameRegexAddDialog';

interface SettingsNameRegexAddDialogProperties {
  readonly onClose?: () => void;
  readonly values: string[];
}

export default React.memo(SettingsNameRegexAddDialog);
