import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";

import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const ChannelGroupDeleteDialog = (props: ChannelGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);
  const [infoMessage, setInfoMessage] = React.useState('');

  const [channelGroupsDeleteChannelGroupMutation] = StreamMasterApi.useChannelGroupsDeleteChannelGroupMutation();

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
    for (const group of selectedChannelGroups.filter((a) => !a.isReadOnly)) {

      const data = {} as StreamMasterApi.DeleteChannelGroupRequest;
      data.groupName = group.name;
      groupNames.push(group.name);
      promises.push(
        channelGroupsDeleteChannelGroupMutation(data)
          .then(() => {

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
        header={`Delete "${selectedChannelGroups.filter((a) => !a.isReadOnly).length < 2 ? selectedChannelGroups.filter((a) => !a.isReadOnly)[0] ? selectedChannelGroups.filter((a) => !a.isReadOnly)[0].name + '" Group ?' : ' Group ?' : selectedChannelGroups.filter((a) => !a.isReadOnly).length + ' Groups ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button
                icon="pi pi-times "
                label="Cancel"
                onClick={(() => ReturnToParent())}
                rounded
                severity="warning"
              />
              <Button
                icon="pi pi-check"
                label="Delete"
                onClick={async () => await deleteGroup()}
                rounded
                severity="success"
              />
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

type ChannelGroupDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  onDelete?: (results: string[] | undefined) => void;
  onHide?: () => void;
  value?: StreamMasterApi.ChannelGroupDto[] | undefined;
};

export default React.memo(ChannelGroupDeleteDialog);
