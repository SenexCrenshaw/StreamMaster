
import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Button } from "primereact/button";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { getTopToolOptions } from "../common/common";
import { InputText } from "primereact/inputtext";

const ChannelGroupEditDialog = (props: ChannelGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [newGroupName, setNewGroupName] = React.useState('');

  const ReturnToParent = React.useCallback(() => {
    setNewGroupName('');
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  }, [props]);

  React.useMemo(() => {

    if (props.value) {
      setNewGroupName(props.value.name);
    }

  }, [props.value]);

  const changeGroupName = () => {
    setBlock(true);
    if (!newGroupName || !props.value) {
      ReturnToParent();
      return;
    }

    const tosend = {} as StreamMasterApi.UpdateChannelGroupRequest;

    tosend.groupName = props.value?.name;
    tosend.newGroupName = newGroupName;

    Hub.UpdateChannelGroup(tosend).then(() => {
      setInfoMessage('Channel Group Renamed Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Rename Error: ' + e.message);
    });

  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Rename Group"
        infoMessage={infoMessage}
        onClose={() => { setInfoMessage(''); }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3>Rename Group</h3>
            <InputText
              autoFocus
              className="withpadding p-inputtext-sm w-full"
              onChange={(e) => setNewGroupName(e.target.value)}
              placeholder="Group Name"
              value={newGroupName}
            />
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button
                icon="pi pi-times "
                label="Cancel"
                onClick={(() => ReturnToParent())}
                rounded
                severity="warning"
              />
              <Button
                icon="pi pi-check"
                label="Rename"
                onClick={changeGroupName}
                rounded
                severity="success"
              />
            </div>
          </div>
        </div >
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-pencil"
        onClick={() => setShowOverlay(true)}
        rounded
        size="small"
        text
        tooltip="Rename Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

ChannelGroupEditDialog.displayName = 'ChannelGroupEditDialog';
ChannelGroupEditDialog.defaultProps = {
  value: null,

};

type ChannelGroupEditDialogProps = {
  onClose?: (() => void);
  value?: StreamMasterApi.ChannelGroupDto | undefined;
};

export default React.memo(ChannelGroupEditDialog);
