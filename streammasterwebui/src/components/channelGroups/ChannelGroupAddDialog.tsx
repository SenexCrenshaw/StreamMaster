import React, { useCallback, useEffect, useState } from "react";

import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";

import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { getTopToolOptions } from "../../common/common";
import { type CreateChannelGroupRequest } from "../../store/iptvApi";
import { useChannelGroupsCreateChannelGroupMutation } from "../../store/iptvApi";
import OKButton from "../buttons/OKButton";


// Define the component props
type ChannelGroupAddDialogProps = {
  readonly onAdd?: (value: string) => void;
  readonly onHide?: () => void;
}

const ChannelGroupAddDialog: React.FC<ChannelGroupAddDialogProps> = ({ onAdd, onHide }) => {

  const [overlayState, setOverlayState] = useState<{ message: string, visible: boolean }>({ message: '', visible: false });
  const [block, setBlock] = useState<boolean>(false);
  const [newGroupName, setNewGroupName] = useState<string>('');

  const [channelGroupsCreateChannelGroupMutation] = useChannelGroupsCreateChannelGroupMutation();

  const ReturnToParent = useCallback(() => {
    setOverlayState({ message: '', visible: false });
    setBlock(false);
    onHide?.();
    onAdd?.(newGroupName);
    setNewGroupName('');
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [onAdd, onHide]);

  const addGroup = useCallback(() => {
    setBlock(true);

    if (!newGroupName) {
      ReturnToParent();

      return;
    }

    const requestData: CreateChannelGroupRequest = {
      groupName: newGroupName,
      isReadOnly: false,
      rank: 0
    };

    channelGroupsCreateChannelGroupMutation(requestData)
      .then(() => setOverlayState({ ...overlayState, message: 'Channel Group Added Successfully' }))
      .catch((e) => setOverlayState({ ...overlayState, message: 'Channel Group Add Error: ' + e.message }));

  }, [ReturnToParent, channelGroupsCreateChannelGroupMutation, newGroupName, overlayState]);

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if ((event.code === 'Enter' || event.code === 'NumpadEnter') && newGroupName !== "") {
        event.preventDefault();
        addGroup();
      }
    };

    document.addEventListener('keydown', handleKeyDown);

    return () => {
      document.removeEventListener('keydown', handleKeyDown);
    };

  }, [addGroup, newGroupName]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header='Add Group'
        infoMessage={overlayState.message}
        onClose={ReturnToParent}
        show={overlayState.visible}
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
              <OKButton onClick={addGroup} />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-plus"
        onClick={() => setOverlayState({ ...overlayState, visible: true })}
        rounded
        severity="success"
        size="small"
        style={{ maxHeight: "2rem", maxWidth: "2rem" }}
        tooltip="Add Group"
        tooltipOptions={getTopToolOptions}
      />
    </>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default React.memo(ChannelGroupAddDialog);
