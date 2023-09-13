
import { memo, useCallback, useEffect, useMemo, useState } from "react";
import { useSelectedChannelGroups } from "../../app/slices/useSelectedChannelGroups";
import { GetMessage } from "../../common/common";
import { UpdateChannelGroup } from "../../smAPI/ChannelGroups/ChannelGroupsMutateAPI";
import { type ChannelGroupDto, type UpdateChannelGroupRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import EditButton from "../buttons/EditButton";
import TextInput from "../inputs/TextInput";


type ChannelGroupEditDialogProps = {
  readonly cgid: string;
  readonly onClose?: ((newName: string) => void);
  readonly value?: ChannelGroupDto | undefined;
};

const ChannelGroupEditDialog = ({ cgid, onClose, value }: ChannelGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [newGroupName, setNewGroupName] = useState('');

  const [channelGroupDto, setChannelGroupDto] = useState<ChannelGroupDto>();
  const { selectedChannelGroups } = useSelectedChannelGroups(cgid);

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

    if (selectedChannelGroups && selectedChannelGroups.length > 0) {
      setNewGroupName(selectedChannelGroups[0].name);
      // if (props.value.regexMatch !== null)
      //   setRegex(props.value.regexMatch);
    }

  }, [channelGroupDto, selectedChannelGroups]);

  const changeGroupName = useCallback(() => {
    setBlock(true);

    const cg = selectedChannelGroups && selectedChannelGroups.length > 0 ? selectedChannelGroups[0] : null;

    if (!newGroupName || !cg?.id) {
      ReturnToParent();

      return;
    }

    const toSend = {} as UpdateChannelGroupRequest;

    toSend.channelGroupId = cg.id;
    toSend.newGroupName = newGroupName;

    UpdateChannelGroup(toSend).then(() => {
      setInfoMessage('Channel Group Edit Successfully');

      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    }).catch((e: any) => {
      setInfoMessage('Channel Group Edit Error: ' + e.message);
    });
    setNewGroupName('');
  }, [ReturnToParent, newGroupName, selectedChannelGroups]);


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
        <div className="flex grid justify-content-center w-full">
          <div className="flex col-12 mt-3 gap-2 justify-content-start">
            <TextInput onChange={(e) => setNewGroupName(e)} placeHolder="Group Name" value={newGroupName} />
          </div>
          <div className="flex col-12 mt-3 gap-2 justify-content-end">
            <EditButton label='Edit Group' onClick={() => changeGroupName()} tooltip='Edit Group' />
          </div>
        </div >
      </InfoMessageOverLayDialog>

      <EditButton
        iconFilled={false}
        onClick={() => setShowOverlay(true)}
        tooltip='Edit Group' />
    </>
  );
}

ChannelGroupEditDialog.displayName = 'ChannelGroupEditDialog';

export default memo(ChannelGroupEditDialog);
