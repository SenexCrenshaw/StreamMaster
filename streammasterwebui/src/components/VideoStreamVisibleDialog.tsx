import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Button } from "primereact/button";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { getTopToolOptions } from "../common/common";

const VideoStreamVisibleDialog = (props: VideoStreamVisibleDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [selectedVideoStreams, setSelectedVideoStreams] = React.useState<StreamMasterApi.VideoStreamDto[]>([] as StreamMasterApi.VideoStreamDto[]);

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  }, [props]);

  React.useMemo(() => {

    if (props.values !== null && props.values !== undefined) {
      setSelectedVideoStreams(props.values);
    }

  }, [props.values]);

  const onVisiblesClick = React.useCallback(async () => {
    setBlock(true);
    if (setSelectedVideoStreams.length === 0) {
      ReturnToParent();
      return;
    }

    const tosend = {} as StreamMasterApi.UpdateVideoStreamsRequest;

    tosend.videoStreamUpdates = selectedVideoStreams.map((a) => {
      return {
        id: a.id,
        isHidden: !a.isHidden
      } as StreamMasterApi.UpdateVideoStreamRequest;
    });


    Hub.UpdateVideoStreams(tosend)
      .then(() => {

        setInfoMessage('Set Stream Visibilty Successfully');

      }
      ).catch((error) => {
        setInfoMessage('Set Stream Visibilty Error: ' + error.message);
      });

  }, [ReturnToParent, selectedVideoStreams]);

  if (props.skipOverLayer === true) {
    return (
      <Button
        disabled={selectedVideoStreams.length === 0}
        icon="pi pi-power-off"
        onClick={async () => await onVisiblesClick()}
        rounded
        severity="info"
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
        header={`Toggle visibility for ${selectedVideoStreams.length < 2 ? selectedVideoStreams.length + ' Stream ?' : selectedVideoStreams.length + ' Streams ?'}`}
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
                label="Change"
                onClick={async () => await onVisiblesClick()}
                rounded
                severity="success"
              />
            </div>
          </div>
        </div>

      </InfoMessageOverLayDialog >

      <Button
        disabled={selectedVideoStreams.length === 0}
        icon="pi pi-power-off"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="info"
        size="small"
        text={props.iconFilled !== true}
        tooltip="Set Visibilty"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

VideoStreamVisibleDialog.displayName = 'VideoStreamVisibleDialog';
VideoStreamVisibleDialog.defaultProps = {
  iconFilled: true,
  values: null,
}

type VideoStreamVisibleDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  skipOverLayer?: boolean | undefined;
  values?: StreamMasterApi.VideoStreamDto[] | undefined;
};

export default React.memo(VideoStreamVisibleDialog);
