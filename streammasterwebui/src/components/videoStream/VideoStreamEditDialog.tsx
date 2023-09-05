
import { useState, useEffect, useCallback, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import VideoStreamPanel from "../../features/videoStreamPanel/VideoStreamPanel";
import { Button } from "primereact/button";

const VideoStreamEditDialog = (props: VideoStreamEditDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  const [videoStream, setVideoStream] = useState<VideoStreamDto | undefined>(undefined);

  useEffect(() => {
    setVideoStream(props.value);

  }, [props.value]);

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);

    props.onClose?.();
  }, [props]);

  const onEdit = useCallback(async (data: UpdateVideoStreamRequest) => {
    setBlock(true);

    if (data === null || data === undefined) {
      ReturnToParent();

      return;
    }

    videoStreamsUpdateVideoStreamMutation(data)
      .then(() => {

        setInfoMessage('Set Stream Edited Successfully');


      }
      ).catch((error) => {
        setInfoMessage('Set Stream Edited Error: ' + error.message);
      });

  }, [ReturnToParent, videoStreamsUpdateVideoStreamMutation]);


  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Edit Video Stream"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={8}
        show={showOverlay}
      >

        <VideoStreamPanel
          onEdit={async (e) => await onEdit(e)}
          videoStream={videoStream}
        />

      </InfoMessageOverLayDialog>

      <Button
        disabled={!videoStream}
        icon="pi pi-pencil"
        onClick={() =>
          setShowOverlay(true)
        }
        rounded
        size="small"
        text={props.iconFilled !== true}
        tooltip="Edit Stream"
        tooltipOptions={getTopToolOptions}
      />
    </>

  );
}

VideoStreamEditDialog.displayName = 'VideoStreamEditDialog';
VideoStreamEditDialog.defaultProps = {
}

type VideoStreamEditDialogProps = {
  readonly iconFilled?: boolean | undefined;
  readonly onClose?: (() => void);
  readonly value?: VideoStreamDto | undefined;
};

export default memo(VideoStreamEditDialog);
