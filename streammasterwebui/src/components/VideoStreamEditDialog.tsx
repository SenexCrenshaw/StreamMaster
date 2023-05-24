
import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import VideoStreamPanel from "./VideoStreamPanel";

const VideoStreamEditDialog = (props: VideoStreamEditDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  }, [props]);

  const onEdit = React.useCallback(async (data: StreamMasterApi.UpdateVideoStreamRequest) => {
    setBlock(true);
    if (data === null || data === undefined) {
      ReturnToParent();
      return;
    }

    Hub.UpdateVideoStream(data)
      .then((resultData) => {
        if (resultData) {
          setInfoMessage('Set Stream Edited Successfully');
        } else {
          setInfoMessage('Set Stream Edited No Change');
        }
      }
      ).catch((error) => {
        setInfoMessage('Set Stream Edited Error: ' + error.message);
      });

  }, [ReturnToParent]);


  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Edit Video Stream"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={8}
        show={showOverlay}
      >

        <VideoStreamPanel
          onClose={() => ReturnToParent()}
          onEdit={async (e) => await onEdit(e)}
          videoStream={props.value}
        />

      </InfoMessageOverLayDialog>

      <Button
        disabled={!props.value}
        icon="pi pi-pencil"
        onClick={() => setShowOverlay(true)}
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
  value?: StreamMasterApi.VideoStreamDto | undefined;
};

export default React.memo(VideoStreamEditDialog);
