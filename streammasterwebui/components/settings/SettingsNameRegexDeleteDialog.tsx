import SMPopUp from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';
import { UpdateSettingRequest } from '@lib/smAPI/smapiTypes';
import { Button } from 'primereact/button';
import React from 'react';

const SettingsNameRegexDeleteDialog = (props: SettingsNameRegexDeleteDialogProperties) => {
  const ReturnToParent = React.useCallback(() => {
    props.onClose?.();
  }, [props]);

  const onSave = React.useCallback(() => {
    if (!props.value || props.value === '') {
      ReturnToParent();

      return;
    }

    const tosend = {} as UpdateSettingRequest;

    tosend.parameters.NameRegex = props.values.filter((a) => a !== props.value);

    UpdateSetting(tosend)
      .then(() => {})
      .catch((error) => {
        Logger.error(`Delete Regex Error: ${error.message}`);
      });
  }, [ReturnToParent, props.value, props.values]);

  return (
    <>
      <SMPopUp icon="pi-minus" header={`Delete Regex: ${props.value}`} tooltip="Delete">
        <div className="m-0 p-0 border-1 border-round surface-border">
          <div className="m-3">
            <div className="card flex mt-3 flex-wrap gap-1 justify-content-center">
              <Button icon="pi pi-times" label="Cancel" onClick={() => ReturnToParent()} rounded severity="warning" />
              <Button icon="pi pi-check" label="Ok" onClick={onSave} rounded severity="success" />
            </div>
          </div>
        </div>
      </SMPopUp>
    </>
  );
};

SettingsNameRegexDeleteDialog.displayName = 'SettingsNameRegexDeleteDialog';

interface SettingsNameRegexDeleteDialogProperties {
  readonly onClose?: () => void;
  readonly value: string;
  readonly values: string[];
}

export default React.memo(SettingsNameRegexDeleteDialog);
