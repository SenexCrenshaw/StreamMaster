import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";

import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const ChannelGroupDeleteDialog = (props: ChannelGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);
  const [infoMessage, setInfoMessage] = React.useState('');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.();
  }, [props]);

  React.useMemo(() => {

    if (props.value != null && props.value !== undefined) {
      setSelectedChannelGroups(props.value);
    }

  }, [props.value]);

  const deleteGroup = React.useCallback(async () => {
    setBlock(true);
    if (selectedChannelGroups.length === 0) {
      ReturnToParent();
      return;
    }

    const ret = [] as number[];
    const promises = [];

    for (const group of selectedChannelGroups.filter((a) => !a.isReadOnly)) {

      const data = {} as StreamMasterApi.DeleteChannelGroupRequest;
      data.groupName = group.name;

      promises.push(
        Hub.DeleteChannelGroup(data)
          .then((returnData) => {
            ret.push(returnData);
          }).catch(() => { })
      );
    }

    const p = Promise.all(promises);

    await p.then(() => {
      if (ret.length === 0) {
        setInfoMessage('Channel Group No changes made');
      } else {
        setInfoMessage('Channel Group Delete Successful');
      }

      props.onChange?.(ret);
    }).catch((error) => {
      setInfoMessage('Channel Group Delete Error: ' + error.message);
    });

  }, [ReturnToParent, props, selectedChannelGroups]);

  const isDisabled = React.useMemo((): boolean => {
    if (props.value === null || props.value === undefined || props.value.length === 0) {
      return true;
    }

    var t = props.value.some((item) => item.isReadOnly !== true);

    return !t;

  }, [props.value]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header={`Delete ${selectedChannelGroups.filter((a) => !a.isReadOnly).length < 2 ? selectedChannelGroups.filter((a) => !a.isReadOnly).length + ' Group ?' : selectedChannelGroups.filter((a) => !a.isReadOnly).length + ' Groups ?'}`}
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
  onChange: null,
  value: null,
};

type ChannelGroupDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  onChange?: ((value: number[]) => void) | null;
  onHide?: () => void;
  value?: StreamMasterApi.ChannelGroupDto[] | undefined;
};

export default React.memo(ChannelGroupDeleteDialog);
