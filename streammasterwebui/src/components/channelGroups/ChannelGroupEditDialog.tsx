
import { memo, useCallback, useEffect, useMemo, useState } from "react";
import { useSelectedItems } from "../../app/slices/useSelectedItemsSlice";
import { GetMessage } from "../../common/common";
import { UpdateChannelGroup } from "../../smAPI/ChannelGroups/ChannelGroupsMutateAPI";
import { type ChannelGroupDto, type UpdateChannelGroupRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import EditButton from "../buttons/EditButton";
import TextInput from "../inputs/TextInput";


type ChannelGroupEditDialogProps = {
  readonly onClose?: ((newName: string) => void);
  readonly value?: ChannelGroupDto | undefined;
};

const ChannelGroupEditDialog = ({ onClose, value }: ChannelGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [newGroupName, setNewGroupName] = useState('');

  const [channelGroupDto, setChannelGroupDto] = useState<ChannelGroupDto>();
  const { selectSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.(newGroupName);
  }, [onClose, newGroupName]);

  useMemo(() => {
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
      // if (props.value.regexMatch !== null)
      //   setRegex(props.value.regexMatch);
    }

  }, [channelGroupDto, selectSelectedItems]);

  const changeGroupName = useCallback(() => {
    console.log('changeGroupName')
    if (!newGroupName || !value) {
      ReturnToParent();
      return;
    }

    setBlock(true);

    const toSend = {} as UpdateChannelGroupRequest;

    toSend.channelGroupId = value.id;
    toSend.newGroupName = newGroupName;

    UpdateChannelGroup(toSend).then(() => {
      setInfoMessage('Channel Group Edit Successfully');

      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    }).catch((e: any) => {
      setInfoMessage('Channel Group Edit Error: ' + e.message);
    });

  }, [ReturnToParent, newGroupName, value]);


  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={GetMessage("edit group")}
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
            <EditButton disabled={value?.name === newGroupName} label='Edit Group' onClick={changeGroupName} tooltip='Edit Group' />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <EditButton
        disabled={!value || value.isReadOnly == true}
        iconFilled={false}
        onClick={() => setShowOverlay(true)}
        tooltip='Edit Group' />
    </>
  );
}

ChannelGroupEditDialog.displayName = 'ChannelGroupEditDialog';

export default memo(ChannelGroupEditDialog);
