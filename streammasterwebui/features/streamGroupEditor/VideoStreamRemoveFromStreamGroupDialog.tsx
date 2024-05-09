import XButton from '@components/buttons/XButton';

import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { memo } from 'react';

interface VideoStreamRemoveFromStreamGroupDialogProperties {
  readonly id: string;
  readonly value?: VideoStreamDto | undefined;
}

const VideoStreamRemoveFromStreamGroupDialog = ({ id, value }: VideoStreamRemoveFromStreamGroupDialogProperties) => {
  const [streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation] = useStreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteMutation();
  const { selectedStreamGroup } = useSelectedStreamGroup(id);

  const removeVideoStream = async () => {
    if (!value || !selectedStreamGroup) {
      return;
    }

    const toSend = {} as StreamGroupVideoStreamsSyncVideoStreamToStreamGroupDeleteApiArg;

    toSend.streamGroupId = selectedStreamGroup.id;
    toSend.videoStreamId = value.id;

    await streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation(toSend)
      .then(() => {})
      .catch((error) => {
        console.error(`Remove Stream Error: ${error.message}`);
      });
  };

  return (
    <div className="flex">
      <XButton iconFilled={false} onClick={async () => await removeVideoStream()} />
    </div>
  );
};

VideoStreamRemoveFromStreamGroupDialog.displayName = 'VideoStreamRemoveFromStreamGroupDialog';

export default memo(VideoStreamRemoveFromStreamGroupDialog);
