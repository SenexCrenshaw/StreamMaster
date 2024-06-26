import XButton from '@components/buttons/XButton';

import { memo } from 'react';

interface VideoStreamRemoveFromVideoStreamDialogProperties {
  readonly value?: ChildVideoStreamDto | undefined;
  readonly videoStreamId: string;
}

const VideoStreamRemoveFromVideoStreamDialog = ({ value, videoStreamId }: VideoStreamRemoveFromVideoStreamDialogProperties) => {
  const removeVideoStream = async () => {
    if (!value) {
      return;
    }

    const toSend = {} as VideoStreamLinksRemoveVideoStreamFromVideoStreamApiArg;

    toSend.parentVideoStreamId = videoStreamId;
    toSend.childVideoStreamId = value.id;

    await RemoveVideoStreamFromVideoStream(toSend)
      .then(() => {})
      .catch((error) => {
        console.error(`Remove Stream Error: ${error.message}`);
      });
  };

  return <XButton onClick={async () => await removeVideoStream()} />;
};

VideoStreamRemoveFromVideoStreamDialog.displayName = 'VideoStreamRemoveFromVideoStreamDialog';

export default memo(VideoStreamRemoveFromVideoStreamDialog);
