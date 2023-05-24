
import React from "react";
import { getTopToolOptions } from "../common/common";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";

import { Button } from "primereact/button";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const VideoStreamDeleteDialog = (props: VideoStreamDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [selectedVideoStreams, setSelectedVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([] as StreamMasterApi.VideoStreamDto[]);
  const [block, setBlock] = React.useState<boolean>(false);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  };

  React.useMemo(() => {

    if (props.values != null && props.values !== undefined) {
      setSelectedVideoStreams(props.values);
    }

  }, [props.values]);

  const deleteVideoStream = async () => {
    setBlock(true);
    if (selectedVideoStreams.length === 0) {
      ReturnToParent();
      return;
    }

    const ret = [] as number[];
    const promises = [];

    for (const stream of selectedVideoStreams) {

      const data = {} as StreamMasterApi.DeleteVideoStreamRequest;

      data.videoStreamId = stream.id;

      promises.push(
        Hub.DeleteVideoStream(data)
          .then((returnData) => {
            ret.push(returnData);
          }).catch(() => { })
      );
    }

    const p = Promise.all(promises);

    await p.then(() => {
      if (ret.length === 0) {
        setInfoMessage('Delete Stream No Changes');
      } else {
        setInfoMessage('Delete Stream Successful');
      }

      props.onChange?.(ret);
    }).catch((error) => {
      setInfoMessage('Delete Stream Error: ' + error.message);
    });

  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header={`Delete ${selectedVideoStreams.length < 2 ? selectedVideoStreams.length + ' Stream ?' : selectedVideoStreams.length + ' Streams ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={6}
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
                onClick={async () => await deleteVideoStream()}
                rounded
                severity="success"
              />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
        disabled={selectedVideoStreams === undefined || selectedVideoStreams.length === 0}
        icon="pi pi-minus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="danger"
        size="small"
        tooltip="Delete Stream"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

VideoStreamDeleteDialog.displayName = 'VideoStreamDeleteDialog';

type VideoStreamDeleteDialogProps = {
  onChange?: ((value: number[]) => void) | null;
  onClose?: (() => void);
  values?: StreamMasterApi.VideoStreamDto[] | undefined;
};

export default React.memo(VideoStreamDeleteDialog);
