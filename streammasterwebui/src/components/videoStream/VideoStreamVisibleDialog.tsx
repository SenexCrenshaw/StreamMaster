
import { useState, useCallback, useMemo, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type VideoStreamDto, type UpdateVideoStreamsRequest, type UpdateVideoStreamRequest } from "../../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamsMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { Button } from "primereact/button";

const VideoStreamVisibleDialog = (props: VideoStreamVisibleDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [selectedVideoStreams, setSelectedVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);

  const [videoStreamsUpdateVideoStreams] = useVideoStreamsUpdateVideoStreamsMutation();


  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  }, [props]);

  useMemo(() => {

    if (props.values !== null && props.values !== undefined) {
      setSelectedVideoStreams(props.values);
    }

  }, [props.values]);

  const onVisiblesClick = useCallback(async () => {
    setBlock(true);
    if (setSelectedVideoStreams.length === 0) {
      ReturnToParent();
      return;
    }

    const tosend = {} as UpdateVideoStreamsRequest;

    tosend.videoStreamUpdates = selectedVideoStreams.map((a) => {
      return {
        id: a.id,
        isHidden: !a.isHidden
      } as UpdateVideoStreamRequest;
    });


    videoStreamsUpdateVideoStreams(tosend)
      .then(() => {

        setInfoMessage('Set Stream Visibilty Successfully');

      }
      ).catch((error) => {
        setInfoMessage('Set Stream Visibilty Error: ' + error.message);
      });

  }, [ReturnToParent, selectedVideoStreams, videoStreamsUpdateVideoStreams]);

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
  skipOverLayer?: boolean;
  values?: VideoStreamDto[] | undefined;
};

export default memo(VideoStreamVisibleDialog);
