

import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import React from "react";
import { getTopToolOptions } from "../../common/common";
import { UpdateSetting } from "../../smAPI/Settings/SettingsMutateAPI";
import type * as StreamMasterApi from '../../store/iptvApi';
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";

const SettingsNameRegexAddDialog = (props: SettingsNameRegexAddDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [regex, setRegex] = React.useState<string | undefined>('');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    setRegex('');
    props.onClose?.();
  }, [props]);


  const onSave = React.useCallback(() => {
    setBlock(true);

    if (!regex || regex === '') {
      ReturnToParent();

      return;
    }

    const tosend = {} as StreamMasterApi.UpdateSettingRequest;

    tosend.nameRegex = [regex, ...props.values]

    UpdateSetting(tosend).then(() => {
      setInfoMessage('Add Regex Successfully');
    }).catch((e) => {
      setInfoMessage('Add Regex Error: ' + e.message);
    });

  }, [ReturnToParent, props.values, regex]);



  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Add Regex"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <InputText
              autoFocus
              className="withpadding p-inputtext-sm w-full"
              onChange={(e) => setRegex(e.target.value)}
              placeholder="Regex"
              value={regex}
            />

            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button
                icon="pi pi-times"
                label="Cancel"
                onClick={(() => ReturnToParent())}
                rounded
                severity="warning"
              />
              <Button
                disabled={!regex || regex === ''}
                icon="pi pi-check"
                label="Ok"
                onClick={onSave}
                rounded
                severity="success"
              />
            </div>

          </div>
        </div >
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-plus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="success"
        size="small"
        text
        tooltip="Edit Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

SettingsNameRegexAddDialog.displayName = 'SettingsNameRegexAddDialog';
SettingsNameRegexAddDialog.defaultProps = {
};

type SettingsNameRegexAddDialogProps = {
  readonly onClose?: (() => void);
  readonly values: string[];
};

export default React.memo(SettingsNameRegexAddDialog);
