import { memo } from "react";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import XButton from "../../components/buttons/XButton";
import { useStreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation, type StreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupApiArg, type VideoStreamDto } from "../../store/iptvApi";

type VideoStreamRemoveFromStreamGroupDialogProps = {
  readonly id: string;
  readonly value?: VideoStreamDto | undefined;
};

const VideoStreamRemoveFromStreamGroupDialog = ({ id, value }: VideoStreamRemoveFromStreamGroupDialogProps) => {
  const [streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation] = useStreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation();
  const { selectedStreamGroup } = useSelectedStreamGroup(id);


  const removeVideoStream = async () => {
    if (!value || !selectedStreamGroup) {
      return;
    }

    const toSend = {} as StreamGroupVideoStreamsRemoveVideoStreamFromStreamGroupApiArg;

    toSend.streamGroupId = selectedStreamGroup.id;
    toSend.videoStreamId = value.id;

    await streamGroupVideoStreamsRemoveVideoStreamFromStreamGroupMutation(toSend).then(() => {
    }).catch((error) => {
      console.error('Remove Stream Error: ' + error.message);
    });

  }

  return (
    <div className='flex'>
      <XButton onClick={async () => await removeVideoStream()} />
    </div>
  );

}

VideoStreamRemoveFromStreamGroupDialog.displayName = 'VideoStreamRemoveFromStreamGroupDialog';

export default memo(VideoStreamRemoveFromStreamGroupDialog);

