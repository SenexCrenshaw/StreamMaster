import { useVideoStreamsUpdateVideoStreamMutation, type UpdateVideoStreamRequest, type VideoStreamDto } from '@lib/iptvApi';
import { memo, useCallback, useEffect, useState } from 'react';

import VideoStreamPanel from '@components/videoStreamPanel/VideoStreamPanel';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import EditButton from '../buttons/EditButton';

const VideoStreamEditDialog = (props: VideoStreamEditDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  const [videoStream, setVideoStream] = useState<VideoStreamDto | undefined>();

  useEffect(() => {
    setVideoStream(props.value);
  }, [props.value]);

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);

    props.onClose?.();
  }, [props]);

  const onEdit = useCallback(
    async (data: UpdateVideoStreamRequest) => {
      setBlock(true);

      if (data === null || data === undefined) {
        ReturnToParent();

        return;
      }

      videoStreamsUpdateVideoStreamMutation(data)
        .then(() => {
          setInfoMessage('Set Stream Edited Successfully');
        })
        .catch((error) => {
          setInfoMessage(`Set Stream Edited Error: ${error.message}`);
        });
    },
    [ReturnToParent, videoStreamsUpdateVideoStreamMutation]
  );

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Edit Video Stream"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={8}
        show={showOverlay}
      >
        <VideoStreamPanel onEdit={async (e) => await onEdit(e)} videoStream={videoStream} />
      </InfoMessageOverLayDialog>

      <EditButton iconFilled={false} onClick={() => setShowOverlay(true)} tooltip="Edit Group" />
    </>
  );
};

VideoStreamEditDialog.displayName = 'VideoStreamEditDialog';

interface VideoStreamEditDialogProperties {
  readonly onClose?: () => void;
  readonly value?: VideoStreamDto | undefined;
}

export default memo(VideoStreamEditDialog);
