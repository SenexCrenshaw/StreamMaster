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
      // console.log('VideoStreamEditDialog onEdit', data);
      setBlock(true);

      if (data === null || data === undefined) {
        // console.log('onUpdateVideoStream data is null');
        ReturnToParent();

        return;
      }

      // console.log('onUpdateVideoStream sending', data);
      // data.streamProxyType=parseInt(data.streamProxyType.toString());
      videoStreamsUpdateVideoStreamMutation(data)
        .then(() => {
          // console.log('onUpdateVideoStream Successful');
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
        header={'Edit Video Stream : ' + videoStream?.user_Tvg_name}
        subHeader={videoStream?.id}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={8}
        show={showOverlay}
      >
        <VideoStreamPanel onEdit={async (e) => await onEdit(e)} videoStream={videoStream} />
      </InfoMessageOverLayDialog>

      <EditButton iconFilled={false} onClick={() => setShowOverlay(true)} tooltip="Edit Stream" />
    </>
  );
};

VideoStreamEditDialog.displayName = 'VideoStreamEditDialog';

interface VideoStreamEditDialogProperties {
  readonly onClose?: () => void;
  readonly value?: VideoStreamDto | undefined;
}

export default memo(VideoStreamEditDialog);
