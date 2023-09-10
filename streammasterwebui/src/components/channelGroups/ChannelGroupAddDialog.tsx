import { memo, useCallback, useEffect, useMemo, useState, type FC } from "react";
import { useChannelGroupsCreateChannelGroupMutation, type CreateChannelGroupRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import AddButton from "../buttons/AddButton";
import TextInput from "../inputs/TextInput";


// Define the component props
type ChannelGroupAddDialogProps = {
  readonly onAdd?: (value: string) => void;
  readonly onHide?: () => void;
}

const ChannelGroupAddDialog: FC<ChannelGroupAddDialogProps> = ({ onAdd, onHide }) => {

  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [newGroupName, setNewGroupName] = useState<string>('');

  const [channelGroupsCreateChannelGroupMutation] = useChannelGroupsCreateChannelGroupMutation();

  const ReturnToParent = useCallback(() => {
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

  const isSaveEnabled = useMemo((): boolean => {

    if (newGroupName && newGroupName !== '') {
      return true;
    }

    return false;

  }, [newGroupName]);

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
        <div className="flex grid justify-content-center align-items-center w-full">
          <div className="flex col-10 mt-1">
            <TextInput label="Group Name" onChange={setNewGroupName} value={newGroupName} />
          </div>
          <div className="flex col-12 justify-content-end">
            <AddButton disabled={!isSaveEnabled} label='Add Group' onClick={addGroup} tooltip='Add Group' />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <AddButton onClick={() => setShowOverlay(true)} tooltip='Add Group' />
    </>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default memo(ChannelGroupAddDialog);
