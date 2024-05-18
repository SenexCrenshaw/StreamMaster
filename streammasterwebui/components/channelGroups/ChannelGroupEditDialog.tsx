import { GetMessage, isFetchBaseQueryError } from '@lib/common/common';

import { memo, useCallback, useEffect, useState } from 'react';

import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import EditButton from '../buttons/EditButton';
import TextInput from '../inputs/TextInput';
import { ChannelGroupDto, UpdateChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { UpdateChannelGroup } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';

interface ChannelGroupEditDialogProperties {
  readonly id: string;
  readonly onClose?: (newName: string) => void;
  readonly value?: ChannelGroupDto | undefined;
}

const ChannelGroupEditDialog = ({ id, onClose, value }: ChannelGroupEditDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [newGroupName, setNewGroupName] = useState('');

  const [channelGroupDto, setChannelGroupDto] = useState<ChannelGroupDto>();
  const { selectedItems, setSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.(newGroupName);
  }, [onClose, newGroupName]);

  useEffect(() => {
    if (value !== null) {
      setChannelGroupDto(value);
    }
  }, [value]);

  useEffect(() => {
    if (channelGroupDto) {
      setNewGroupName(channelGroupDto.Name);
      return;
    }

    if (selectedItems && selectedItems.length > 0) {
      setNewGroupName(selectedItems[0].Name);
    }
  }, [channelGroupDto, selectedItems]);

  const changeGroupName = useCallback(() => {
    if (!newGroupName || !value) {
      ReturnToParent();
      return;
    }

    setBlock(true);

    const toSend = {} as UpdateChannelGroupRequest;

    toSend.ChannelGroupId = value.Id;
    toSend.NewGroupName = newGroupName;

    UpdateChannelGroup(toSend)
      .then(() => {
        console.log(selectedItems);

        const updatedselectedItems = selectedItems.map((item) => (item.Id === value.Id ? { ...item, name: newGroupName } : item));
        if (updatedselectedItems) {
          setSelectedItems(updatedselectedItems);
        }
        setInfoMessage('Channel Group Edit Successfully');
      })
      .catch((error) => {
        if (isFetchBaseQueryError(error)) {
          setInfoMessage(`Delete Error: ${error.status}`);
        }
      });
  }, [ReturnToParent, newGroupName, selectedItems, setSelectedItems, value]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={GetMessage('edit group')}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="flex grid justify-content-center align-items-center w-full">
          <div className="flex col-10 mt-1">
            <TextInput onChange={setNewGroupName} onEnter={changeGroupName} placeHolder="Group Name" value={newGroupName} />
          </div>
          <div className="flex col-12 justify-content-end">
            <EditButton disabled={value?.Name === newGroupName} label="Edit Group" onClick={changeGroupName} tooltip="Edit Group" />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <EditButton disabled={!value || value.IsReadOnly === true} iconFilled={false} onClick={() => setShowOverlay(true)} tooltip="Edit Group" />
    </>
  );
};

ChannelGroupEditDialog.displayName = 'ChannelGroupEditDialog';

export default memo(ChannelGroupEditDialog);
