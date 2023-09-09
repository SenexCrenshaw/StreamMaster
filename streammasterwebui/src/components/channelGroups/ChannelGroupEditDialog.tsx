import { InputText } from "primereact/inputtext";
import React from "react";
import { GetMessage } from "../../common/common";
import { useChannelGroupsUpdateChannelGroupMutation, type ChannelGroupDto, type UpdateChannelGroupRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import EditButton from "../buttons/EditButton";

const ChannelGroupEditDialog = (props: ChannelGroupEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [newGroupName, setNewGroupName] = React.useState('');

  const [channelGroupsUpdateChannelGroupMutation] = useChannelGroupsUpdateChannelGroupMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.(newGroupName);
  }, [props, newGroupName]);


  React.useEffect(() => {

    if (props.value) {
      setNewGroupName(props.value.name);
      // if (props.value.regexMatch !== null)
      //   setRegex(props.value.regexMatch);
    }

  }, [props.value]);

  const changeGroupName = React.useCallback(() => {
    setBlock(true);

    if (!newGroupName || !props.value?.id) {
      ReturnToParent();

      return;
    }

    const toSend = {} as UpdateChannelGroupRequest;

    toSend.channelGroupId = props.value.id;
    toSend.newGroupName = newGroupName;

    channelGroupsUpdateChannelGroupMutation(toSend).then(() => {
      setInfoMessage('Channel Group Edit Successfully');

      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    }).catch((e: any) => {
      setInfoMessage('Channel Group Edit Error: ' + e.message);
    });
    setNewGroupName('');
  }, [ReturnToParent, channelGroupsUpdateChannelGroupMutation, newGroupName, props.value]);


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
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3>{GetMessage("edit group")}</h3>
            <InputText
              autoFocus
              className="withpadding p-inputtext-sm w-full mb-1"
              onChange={(e) => setNewGroupName(e.target.value)}
              placeholder="Group Name"
              value={newGroupName}
            />

            <div className="flex col-12 mt-3 gap-2 justify-content-end">
              <EditButton label='Edit Group' onClick={() => changeGroupName()} tooltip='Edit Group' />
            </div>

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
ChannelGroupEditDialog.defaultProps = {
  value: null,

};

type ChannelGroupEditDialogProps = {
  readonly onClose?: ((newName: string) => void);
  readonly value?: ChannelGroupDto | undefined;
};

export default React.memo(ChannelGroupEditDialog);
