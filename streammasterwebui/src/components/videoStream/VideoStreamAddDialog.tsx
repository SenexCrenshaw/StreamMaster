import { Button } from "primereact/button";
import { useState, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type CreateVideoStreamRequest } from "../../store/iptvApi";
import { CreateVideoStream } from "../../store/signlar_functions";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import VideoStreamPanel from "./VideoStreamPanel";

const VideoStreamAddDialog = (props: VideoStreamAddDialogProps) => {
  const [block, setBlock] = useState<boolean>(false);
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  };

  const onSave = async (data: CreateVideoStreamRequest) => {

    if (data === null || data === undefined) {
      return;
    }

    setBlock(true);

    await CreateVideoStream(data)
      .then(() => {

        setInfoMessage('Add Stream Successful');

      }).catch((error) => {
        setInfoMessage('Add Stream Error: ' + error.message);
      }
      );
  };

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Add Video Stream"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={8}
        show={showOverlay}
      >

        <VideoStreamPanel
          group={props.group}
          onSave={async (e) => await onSave(e)}
        />

      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-plus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="success"
        size="small"
        tooltip="Add Custom Stream"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
};

VideoStreamAddDialog.displayName = 'VideoStreamAddDialog';
VideoStreamAddDialog.defaultProps = {

};
type VideoStreamAddDialogProps = {
  group?: string | undefined;
  onClose?: (() => void);
};

export default memo(VideoStreamAddDialog);
