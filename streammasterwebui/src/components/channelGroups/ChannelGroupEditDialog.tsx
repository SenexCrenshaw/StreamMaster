import React from "react";
import { Button } from "primereact/button";
import { InputText } from "primereact/inputtext";
import { type ChannelGroupDto, type UpdateChannelGroupRequest } from "../../store/iptvApi";
import { useChannelGroupsUpdateChannelGroupMutation } from "../../store/iptvApi";
import { GetMessage, getTopToolOptions } from "../../common/common";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";

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

    if (!newGroupName || !props.value) {
      ReturnToParent();

      return;
    }

    const toSend = {} as UpdateChannelGroupRequest;

    toSend.channelGroupName = props.value?.name;
    toSend.newGroupName = newGroupName;

    channelGroupsUpdateChannelGroupMutation(toSend).then(() => {
      setInfoMessage('Channel Group Edit Successfully');

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

            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button
                icon="pi pi-check"
                label={GetMessage('ok')}
                onClick={changeGroupName}
                rounded
                severity="success"
              />
            </div>

          </div>
        </div >
      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-pencil"
        onClick={() => setShowOverlay(true)}
        rounded
        size="small"
        text
        tooltip="Edit Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

ChannelGroupEditDialog.displayName = 'ChannelGroupEditDialog';
ChannelGroupEditDialog.defaultProps = {
  value: null,

};

type ChannelGroupEditDialogProps = {
  onClose?: ((newName: string) => void);
  value?: ChannelGroupDto | undefined;
};

export default React.memo(ChannelGroupEditDialog);
