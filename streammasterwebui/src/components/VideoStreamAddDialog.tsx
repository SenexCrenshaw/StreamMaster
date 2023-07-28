
import React from 'react';
import type * as StreamMasterApi from '../store/iptvApi';
import VideoStreamPanel from './VideoStreamPanel';
import { Button } from 'primereact/button';
import { getTopToolOptions } from '../common/common';
import InfoMessageOverLayDialog from './InfoMessageOverLayDialog';
import * as Hub from "../store/signlar_functions";

const VideoStreamAddDialog = (props: VideoStreamAddDialogProps) => {
  const [block, setBlock] = React.useState<boolean>(false);
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  };

  const onSave = async (data: StreamMasterApi.AddVideoStreamRequest) => {

    if (data === null || data === undefined) {
      return;
    }

    setBlock(true);

    await Hub.AddVideoStream(data)
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
        header="Add Video Stream"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={8}
        show={showOverlay}
      >

        <VideoStreamPanel
          group={props.group}
          onClose={() => ReturnToParent()}
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

export default React.memo(VideoStreamAddDialog);
