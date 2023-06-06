import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { AddChannelGroup } from "../store/signlar_functions";
import { getTopToolOptions } from "../common/common";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const ChannelGroupAddDialog = (props: ChannelGroupAddDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [newGroupName, setNewGroupName] = React.useState('');
  const [infoMessage, setInfoMessage] = React.useState('');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.(newGroupName);
    setNewGroupName('');
  }, [newGroupName, props]);

  const addGroup = React.useCallback(() => {
    setBlock(true);
    if (!newGroupName) {
      ReturnToParent();
      return;
    }

    const data = {} as StreamMasterApi.AddChannelGroupRequest;
    data.groupName = newGroupName;

    AddChannelGroup(data).then(() => {
      setInfoMessage('Channel Group Added Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Add Error: ' + e.message);
    });

  }, [ReturnToParent, newGroupName]);

  React.useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        event.preventDefault();
        if (newGroupName !== "") {
          addGroup();
        }
      }

    };

    document.addEventListener('keydown', callback);
    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [addGroup, newGroupName]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header='Add Group'
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
                label="Add"
                onClick={addGroup}
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
        style={{
          ...{
            maxHeight: "2rem",
            maxWidth: "2rem"
          }
        }}
        tooltip="Add Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
};


ChannelGroupAddDialog.displayName = 'Play List Editor';
ChannelGroupAddDialog.defaultProps = {

};

export type ChannelGroupAddDialogProps = {
  onHide?: (value: string) => void;
};

export default React.memo(ChannelGroupAddDialog);
