import { memo } from "react";
import { type ChildVideoStreamDto, type VideoStreamLinksRemoveVideoStreamFromVideoStreamApiArg } from "../../store/iptvApi";
import { useVideoStreamLinksRemoveVideoStreamFromVideoStreamMutation } from "../../store/iptvApi";
import XButton from "../../components/buttons/XButton";

type VideoStreamRemoveFromVideoStreamDialogProps = {
  readonly value?: ChildVideoStreamDto | undefined;
  readonly videoStreamId: string;
};

const VideoStreamRemoveFromVideoStreamDialog = ({ value, videoStreamId }: VideoStreamRemoveFromVideoStreamDialogProps) => {

  const [videoStreamLinksRemoveVideoStreamFromVideoStreamMutation] = useVideoStreamLinksRemoveVideoStreamFromVideoStreamMutation();

  const removeVideoStream = async () => {
    if (!value) {
      return;
    }

    const toSend = {} as VideoStreamLinksRemoveVideoStreamFromVideoStreamApiArg;

    toSend.parentVideoStreamId = videoStreamId;
    toSend.childVideoStreamId = value.id;

    await videoStreamLinksRemoveVideoStreamFromVideoStreamMutation(toSend).then(() => {

    }).catch((error) => {
      console.error('Remove Stream Error: ' + error.message);
    });

  }

  return (
    <XButton onClick={async () => await removeVideoStream()} />
  );

}

VideoStreamRemoveFromVideoStreamDialog.displayName = 'VideoStreamRemoveFromVideoStreamDialog';

export default memo(VideoStreamRemoveFromVideoStreamDialog);

