import { getTopToolOptions } from '@/lib/common/common';
import { type UpdateSettingRequest } from '@/lib/iptvApi';
import { UpdateSetting } from '@/lib/smAPI/Settings/SettingsMutateAPI';
import { Button } from 'primereact/button';
import React from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';

const SettingsNameRegexDeleteDialog = (props: SettingsNameRegexDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);

    props.onClose?.();
  }, [props]);

  const onSave = React.useCallback(() => {
    setBlock(true);

    if (!props.value || props.value === '') {
      ReturnToParent();

      return;
    }

    const tosend = {} as UpdateSettingRequest;

    tosend.nameRegex = props.values.filter((a) => a !== props.value);

    UpdateSetting(tosend)
      .then(() => {
        setInfoMessage('Add Regex Successfully');
      })
      .catch((e) => {
        setInfoMessage('Add Regex Error: ' + e.message);
      });
  }, [ReturnToParent, props.value, props.values]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header={`Delete Regex: ${props.value}`}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="m-0 p-0 border-1 border-round surface-border">
          <div className="m-3">
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button icon="pi pi-times" label="Cancel" onClick={() => ReturnToParent()} rounded severity="warning" />
              <Button icon="pi pi-check" label="Ok" onClick={onSave} rounded severity="success" />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-minus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="danger"
        size="small"
        text
        tooltip="Delete"
        tooltipOptions={getTopToolOptions}
      />
    </>
  );
};

SettingsNameRegexDeleteDialog.displayName = 'SettingsNameRegexDeleteDialog';

type SettingsNameRegexDeleteDialogProps = {
  readonly onClose?: () => void;
  readonly value: string;
  readonly values: string[];
};

export default React.memo(SettingsNameRegexDeleteDialog);
