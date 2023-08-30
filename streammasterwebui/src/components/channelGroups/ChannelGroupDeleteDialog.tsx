import React from "react";
import { Button } from "primereact/button";
import { type ChannelGroupDto, type DeleteChannelGroupRequest } from "../../store/iptvApi";
import { useChannelGroupsDeleteChannelGroupMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { getTopToolOptions } from "../../common/common";
import DeleteButton from "../buttons/DeleteButton";
import { useChannelGroupToRemove } from "../../app/slices/useChannelGroupToRemove";

type ChannelGroupDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  id: string;
  onDelete?: (results: string[] | undefined) => void;
  onHide?: () => void;
  value?: ChannelGroupDto[] | undefined;
};


const ChannelGroupDeleteDialog = (props: ChannelGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);
  const [infoMessage, setInfoMessage] = React.useState('');

  const { setChannelGroupToRemove } = useChannelGroupToRemove(props.id);


  const [channelGroupsDeleteChannelGroupMutation] = useChannelGroupsDeleteChannelGroupMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.();
  }, [props]);

  React.useMemo(() => {

    if (props.value !== null && props.value !== undefined) {
      setSelectedChannelGroups(props.value);
    }

  }, [props.value]);

  const deleteGroup = React.useCallback(async () => {
    setBlock(true);
    if (selectedChannelGroups.length === 0) {
      ReturnToParent();
      return;
    }

    const promises = [];
    const groupNames = [] as string[];
    for (const group of selectedChannelGroups.filter((a) => a.id !== undefined && !a.isReadOnly)) {

      if (group.id === undefined) {
        continue;
      }

      const data = {} as DeleteChannelGroupRequest;
      data.groupName = group.name;
      groupNames.push(group.name);
      promises.push(
        channelGroupsDeleteChannelGroupMutation(data)
          .then(() => {
            if (group.id !== undefined) {
              setChannelGroupToRemove(group.id);
            }
          }).catch(() => { })
      );
    }

    const p = Promise.all(promises);

    await p.then(() => {

      setInfoMessage('Channel Group Delete Successful');
      props.onDelete?.(groupNames);
    }).catch((error) => {
      setInfoMessage('Channel Group Delete Error: ' + error.message);
      props.onDelete?.(undefined);
    });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedChannelGroups]);

  const isDisabled = React.useMemo((): boolean => {
    if (props.iconFilled !== true) {
      return false;
    }

    if (props.value === null || props.value === undefined || props.value.length === 0) {
      return true;
    }

    var t = props.value.some((item) => item.isReadOnly !== true);

    return !t;

  }, [props.value, props.iconFilled]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={`Delete "${selectedChannelGroups.filter((a) => !a.isReadOnly).length < 2 ? selectedChannelGroups.filter((a) => !a.isReadOnly)[0] ? selectedChannelGroups.filter((a) => !a.isReadOnly)[0].name + '" Group ?' : ' Group ?' : selectedChannelGroups.filter((a) => !a.isReadOnly).length + ' Groups ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <DeleteButton onClick={async () => await deleteGroup()} tooltip="Delete Group" />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
        disabled={isDisabled}
        icon="pi pi-minus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="danger"
        size="small"
        text={props.iconFilled !== true}
        tooltip="Delete Group"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

ChannelGroupDeleteDialog.displayName = 'ChannelGroupDeleteDialog';
ChannelGroupDeleteDialog.defaultProps = {
  iconFilled: true,
  value: null,
};

export default React.memo(ChannelGroupDeleteDialog);
