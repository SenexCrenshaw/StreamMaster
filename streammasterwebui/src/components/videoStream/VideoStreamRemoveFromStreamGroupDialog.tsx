
import { memo } from "react";
import { type StreamGroupVideoStreamsRemoveVideoStreamToStreamGroupApiArg } from "../../store/iptvApi";
import { useStreamGroupVideoStreamsRemoveVideoStreamToStreamGroupMutation } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import DeleteButton from "../buttons/DeleteButton";

const VideoStreamRemoveFromStreamGroupDialog = (props: VideoStreamRemoveFromStreamGroupDialogProps) => {

  const [streamGroupVideoStreamsRemoveVideoStreamToStreamGroupMutation] = useStreamGroupVideoStreamsRemoveVideoStreamToStreamGroupMutation();

  const removeVideoStream = async () => {
    if (!props.value) {
      return;
    }

    console.log(props.value);

    const toSend = {} as StreamGroupVideoStreamsRemoveVideoStreamToStreamGroupApiArg;

    toSend.streamGroupId = props.streamGroupId;
    toSend.videoStreamId = props.value.id;

    await streamGroupVideoStreamsRemoveVideoStreamToStreamGroupMutation(toSend).then(() => {

    }).catch((error) => {
      console.error('Remove Stream Error: ' + error.message);
    });

  }


  return (
    <DeleteButton iconFilled={false} onClick={async () => await removeVideoStream()} tooltip="Remove Stream" />
  );

}

VideoStreamRemoveFromStreamGroupDialog.displayName = 'VideoStreamRemoveFromStreamGroupDialog';

type VideoStreamRemoveFromStreamGroupDialogProps = {
  streamGroupId: number;
  value?: VideoStreamDto | undefined;
};

export default memo(VideoStreamRemoveFromStreamGroupDialog);

