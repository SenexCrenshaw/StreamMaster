import { isFetchBaseQueryError } from '@/lib/common/common';
import { useChannelGroupsCreateChannelGroupMutation, type CreateChannelGroupRequest } from '@lib/iptvApi';
import { memo, useCallback, useMemo, useState, type FC } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import AddButton from '../buttons/AddButton';
import TextInput from '../inputs/TextInput';

const ChannelGroupAddDialog: FC = () => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [newGroupName, setNewGroupName] = useState<string>('');

  const [channelGroupsCreateChannelGroupMutation] = useChannelGroupsCreateChannelGroupMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    setNewGroupName('');
  }, []);

  const addGroup = useCallback(() => {
    setBlock(true);

    if (!newGroupName) {
      ReturnToParent();

      return;
    }

    const requestData: CreateChannelGroupRequest = {
      groupName: newGroupName,
      isReadOnly: false
    };

    channelGroupsCreateChannelGroupMutation(requestData)
      .then(() => {
        setInfoMessage('Channel Group Added Successfully');
      })
      .catch((error: unknown) => {
        if (isFetchBaseQueryError(error)) {
          setInfoMessage(`Delete Error: ${error.status}`);
        }
      });
  }, [ReturnToParent, channelGroupsCreateChannelGroupMutation, newGroupName]);

  const isSaveEnabled = useMemo((): boolean => {
    if (newGroupName && newGroupName !== '') {
      return true;
    }

    return false;
  }, [newGroupName]);

  return (
    <>
      <InfoMessageOverLayDialog blocked={block} closable header="Add Group" infoMessage={infoMessage} onClose={ReturnToParent} show={showOverlay}>
        <div className="flex grid justify-content-center align-items-center w-full">
          <div className="flex col-10 mt-1">
            <TextInput label="Group Name" onChange={setNewGroupName} onEnter={addGroup} value={newGroupName} />
          </div>
          <div className="flex col-12 justify-content-end">
            <AddButton disabled={!isSaveEnabled} label="Add Group" onClick={addGroup} tooltip="Add Group" />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <AddButton onClick={() => setShowOverlay(true)} tooltip="Add Group" />
    </>
  );
};

ChannelGroupAddDialog.displayName = 'Play List Editor';

export default memo(ChannelGroupAddDialog);
