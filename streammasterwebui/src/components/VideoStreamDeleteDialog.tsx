
import React from "react";
import { getTopToolOptions } from "../common/common";
import * as Hub from "../store/signlar_functions";

import { Button } from "primereact/button";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { type DeleteVideoStreamRequest, type VideoStreamDto } from "../store/iptvApi";

const VideoStreamDeleteDialog = (props: VideoStreamDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [selectedVideoStreams, setSelectedVideoStreams] = React.useState<VideoStreamDto[]>([] as VideoStreamDto[]);
  const [block, setBlock] = React.useState<boolean>(false);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  };

  React.useMemo(() => {

    if (props.values !== null && props.values !== undefined) {
      setSelectedVideoStreams(props.values);
    }

  }, [props.values]);

  React.useMemo(() => {

    if (props.value !== null && props.value !== undefined) {
      setSelectedVideoStreams([props.value]);
    }

  }, [props.value]);

  const deleteVideoStream = async () => {
    setBlock(true);
    if (selectedVideoStreams.length === 0) {
      ReturnToParent();
      return;
    }

    const promises = [];

    for (const stream of selectedVideoStreams) {

      const data = {} as DeleteVideoStreamRequest;

      data.id = stream.id;

      promises.push(
        Hub.DeleteVideoStream(data)
          .then(() => {

          }).catch(() => { })
      );
    }

    const p = Promise.all(promises);

    await p.then(() => {

      setInfoMessage('Delete Stream Successful');

    }).catch((error) => {
      setInfoMessage('Delete Stream Error: ' + error.message);
    });

  }

  if (props.skipOverLayer === true) {
    return (
      <Button
        disabled={selectedVideoStreams.length === 0 || selectedVideoStreams[0].isUserCreated !== true}
        icon="pi pi-minus"
        onClick={async () => await deleteVideoStream()}
        rounded
        severity="danger"
        size="small"
        text={props.iconFilled !== true}
        tooltip="Set Visibilty"
        tooltipOptions={getTopToolOptions}
      />
    );
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
        text={props.iconFilled !== true}
        tooltip="Delete Stream"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

VideoStreamDeleteDialog.displayName = 'VideoStreamDeleteDialog';

type VideoStreamDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  skipOverLayer?: boolean | undefined;
  value?: VideoStreamDto | undefined;
  values?: VideoStreamDto[] | undefined;
};

export default React.memo(VideoStreamDeleteDialog);
