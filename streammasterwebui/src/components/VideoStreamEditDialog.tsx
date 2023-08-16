
import React from "react";

import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import VideoStreamPanel from "./VideoStreamPanel";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamMutation } from "../store/iptvApi";

const VideoStreamEditDialog = (props: VideoStreamEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  const [videoStream, setVideoStream] = React.useState<VideoStreamDto | undefined>(undefined);

  React.useEffect(() => {
    setVideoStream(props.value);

  }, [props.value]);

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);

    props.onClose?.();
  }, [props]);

  const onEdit = React.useCallback(async (data: UpdateVideoStreamRequest) => {
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
          onClose={() => ReturnToParent()}
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
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  value?: VideoStreamDto | undefined;
};

export default React.memo(VideoStreamEditDialog);
