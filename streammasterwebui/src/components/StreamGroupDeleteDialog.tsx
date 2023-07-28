import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";

import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const StreamGroupDeleteDialog = (props: StreamGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedStreamGroup, setSelectedStreamGroup] = React.useState<StreamMasterApi.StreamGroupDto>({} as StreamMasterApi.StreamGroupDto);
  const [infoMessage, setInfoMessage] = React.useState('');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onHide?.();
  }, [props]);

  React.useMemo(() => {

    if (props.value !== null && props.value !== undefined) {
      setSelectedStreamGroup(props.value);
    }

  }, [props.value]);

  const deleteStreamGroup = React.useCallback(async () => {
    setBlock(true);
    if (selectedStreamGroup === undefined) {
      ReturnToParent();
      return;
    }

    const promises = [];

    const data = {} as StreamMasterApi.DeleteStreamGroupRequest;
    data.id = selectedStreamGroup.id;

    promises.push(
      Hub.DeleteStreamGroup(data)
        .then(() => {

        }).catch(() => { })
    );


    const p = Promise.all(promises);

    await p.then(() => {

      setInfoMessage('Stream Group No changes made');

    }).catch((error) => {
      setInfoMessage('Stream Group Delete Error: ' + error.message);
    });

  }, [ReturnToParent, selectedStreamGroup]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header={`Delete "${selectedStreamGroup.name}" ?`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />
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
                onClick={async () => await deleteStreamGroup()}
                rounded
                severity="success"
              />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
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

StreamGroupDeleteDialog.displayName = 'StreamGroupDeleteDialog';
StreamGroupDeleteDialog.defaultProps = {
  iconFilled: true,
  value: null,
};

type StreamGroupDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  onHide?: () => void;
  value?: StreamMasterApi.StreamGroupDto | undefined;
};

export default React.memo(StreamGroupDeleteDialog);
