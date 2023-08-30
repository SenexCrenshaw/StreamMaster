import { memo } from "react";
import { type StreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupApiArg } from "../../store/iptvApi";
import { useStreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation, type VideoStreamDto } from "../../store/iptvApi";
import XButton from "../../components/buttons/XButton";
import { useStreamToRemove } from "../../app/slices/useStreamToRemove";

type VideoStreamRemoveFromStreamGroupDialogProps = {
  id: string;
  streamGroupId: number;
  value?: VideoStreamDto | undefined;
};

const VideoStreamRemoveFromStreamGroupDialog = ({ id, streamGroupId, value }: VideoStreamRemoveFromStreamGroupDialogProps) => {
  const [streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation] = useStreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation();
  const { setStreamToRemove } = useStreamToRemove(id);

  const removeVideoStream = async () => {
    if (!value) {
      return;
    }

    const toSend = {} as StreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupApiArg;

    toSend.streamGroupId = streamGroupId;
    toSend.videoStreamId = value.id;

    await streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation(toSend).then(() => {
      setStreamToRemove(value.id)
    }).catch((error) => {
      console.error('Remove Stream Error: ' + error.message);
    });

  }

  return (
    <XButton onClick={async () => await removeVideoStream()} />
  );

}

VideoStreamRemoveFromStreamGroupDialog.displayName = 'VideoStreamRemoveFromStreamGroupDialog';

export default memo(VideoStreamRemoveFromStreamGroupDialog);

