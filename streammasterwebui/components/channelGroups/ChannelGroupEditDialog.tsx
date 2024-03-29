import { GetMessage, isFetchBaseQueryError } from '@lib/common/common';

import { memo, useCallback, useEffect, useState } from 'react';

import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';

import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import EditButton from '../buttons/EditButton';
import TextInput from '../inputs/TextInput';

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
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');

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
      setNewGroupName(channelGroupDto.name);
      return;
    }

    if (selectSelectedItems && selectSelectedItems.length > 0) {
      setNewGroupName(selectSelectedItems[0].name);
    }
  }, [channelGroupDto, selectSelectedItems]);

  const changeGroupName = useCallback(() => {
    if (!newGroupName || !value) {
      ReturnToParent();
      return;
    }

    setBlock(true);

    const toSend = {} as UpdateChannelGroupRequest;

    toSend.channelGroupId = value.id;
    toSend.newGroupName = newGroupName;

    UpdateChannelGroup(toSend)
      .then(() => {
        console.log(selectSelectedItems);

        const updatedSelectSelectedItems = selectSelectedItems.map((item) => (item.id === value.id ? { ...item, name: newGroupName } : item));
        if (updatedSelectSelectedItems) {
          setSelectSelectedItems(updatedSelectSelectedItems);
        }
        setInfoMessage('Channel Group Edit Successfully');
      })
      .catch((error) => {
        if (isFetchBaseQueryError(error)) {
          setInfoMessage(`Delete Error: ${error.status}`);
        }
      });
  }, [ReturnToParent, newGroupName, selectSelectedItems, setSelectSelectedItems, value]);

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
            <EditButton disabled={value?.name === newGroupName} label="Edit Group" onClick={changeGroupName} tooltip="Edit Group" />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <EditButton disabled={!value || value.isReadOnly === true} iconFilled={false} onClick={() => setShowOverlay(true)} tooltip="Edit Group" />
    </>
  );
};

ChannelGroupEditDialog.displayName = 'ChannelGroupEditDialog';

export default memo(ChannelGroupEditDialog);
