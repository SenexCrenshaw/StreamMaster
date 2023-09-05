import { memo } from "react";
import { useStreamGroupVideoStreamsAddVideoStreamToStreamGroupMutation, type StreamGroupVideoStreamsAddVideoStreamToStreamGroupApiArg } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import RightArrowButton from "../../components/buttons/RightArrowButton";

type VideoStreamAddToStreamGroupDialogProps = {
  readonly id: string;
  readonly value?: VideoStreamDto | undefined;
};

const VideoStreamAddToStreamGroupDialog = ({ id, value }: VideoStreamAddToStreamGroupDialogProps) => {
  const [streamGroupVideoStreamsAddVideoStreamToStreamGroupMutation] = useStreamGroupVideoStreamsAddVideoStreamToStreamGroupMutation();
  const { selectedStreamGroup } = useSelectedStreamGroup(id);


  const addVideoStream = async () => {
    if (!value || !selectedStreamGroup) {
      return;
    }

    const toSend = {} as StreamGroupVideoStreamsAddVideoStreamToStreamGroupApiArg;

    toSend.streamGroupId = selectedStreamGroup.id;
    toSend.videoStreamId = value.id;

    await streamGroupVideoStreamsAddVideoStreamToStreamGroupMutation(toSend).then(() => {
    }).catch((error) => {
      console.error('Add Stream Error: ' + error.message);
    });

  }

  return (
    <RightArrowButton onClick={async () => await addVideoStream()} />
  );

}

VideoStreamAddToStreamGroupDialog.displayName = 'VideoStreamAddToStreamGroupDialog';

export default memo(VideoStreamAddToStreamGroupDialog);

