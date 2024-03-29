import { getTopToolOptions } from '@lib/common/common';

import { Button } from 'primereact/button';
import { memo, useState } from 'react';

import VideoStreamPanel from '@components/videoStreamPanel/VideoStreamPanel';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';

interface VideoStreamAddDialogProperties {
  readonly group?: string | undefined;
  readonly onClose?: () => void;
}

const VideoStreamAddDialog = ({ group, onClose }: VideoStreamAddDialogProperties) => {
  const [block, setBlock] = useState<boolean>(false);
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [videoStreamsCreateVideoStreamMutation] = useVideoStreamsCreateVideoStreamMutation();

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  };

  const onSave = async (data: CreateVideoStreamRequest) => {
    if (data === null || data === undefined) {
      return;
    }

    setBlock(true);

    await videoStreamsCreateVideoStreamMutation(data)
      .then(() => {
        setInfoMessage('Add Stream Successful');
      })
      .catch((error) => {
        setInfoMessage(`Add Stream Error: ${error.message}`);
      });
  };

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Add Video Stream"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={8}
        show={showOverlay}
      >
        <VideoStreamPanel group={group} onSave={async (e) => await onSave(e)} />
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

export default memo(VideoStreamAddDialog);
