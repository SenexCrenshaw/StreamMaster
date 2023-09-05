import React, { useCallback, useEffect, useState } from "react";
import { InputText } from "primereact/inputtext";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { type CreateChannelGroupRequest } from "../../store/iptvApi";
import { useChannelGroupsCreateChannelGroupMutation } from "../../store/iptvApi";
import AddButton from "../buttons/AddButton";


// Define the component props
type ChannelGroupAddDialogProps = {
  readonly onAdd?: (value: string) => void;
  readonly onHide?: () => void;
}

const ChannelGroupAddDialog: React.FC<ChannelGroupAddDialogProps> = ({ onAdd, onHide }) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [newGroupName, setNewGroupName] = useState<string>('');

  const [channelGroupsCreateChannelGroupMutation] = useChannelGroupsCreateChannelGroupMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
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
      .then(() => {
        setInfoMessage('Channel Group Added Successfully');
      })
      .catch((e) => {
        setInfoMessage('Stream Group Add Error: ' + e.message);
      });

  }, [ReturnToParent, channelGroupsCreateChannelGroupMutation, newGroupName]);

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
        infoMessage={infoMessage}
        onClose={ReturnToParent}
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
              <AddButton label='Add Group' onClick={addGroup} tooltip='Add Group' />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <AddButton onClick={() => setShowOverlay(true)} tooltip='Add Group' />
    </>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default React.memo(ChannelGroupAddDialog);
