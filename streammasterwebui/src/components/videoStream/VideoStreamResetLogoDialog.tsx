import { useCallback, memo } from "react";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
import { UpdateVideoStream } from "../../store/signlar_functions";
import ResetButton from "../buttons/ResetButton";

const VideoStreamResetLogoDialog = (props: VideoStreamResetLogoDialogProps) => {

  const ReturnToParent = useCallback(() => {
    props.onClose?.();
  }, [props]);

  const onResetLogo = useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined) {
      ReturnToParent();
      return;
    }

    const toSend = {} as UpdateVideoStreamRequest;
    toSend.id = props.value?.id;
    toSend.tvg_logo = props.value.tvg_logo;

    await UpdateVideoStream(toSend)
      .then(() => {

      }
      ).catch(() => {

      });

  }, [ReturnToParent, props.value]);

  return (
    <ResetButton
      disabled={props.value?.tvg_logo === props.value?.user_Tvg_logo}
      iconFilled={props.iconFilled}
      onClick={async () =>
        await onResetLogo()
      }
      tooltip="Reset Logo From File"
    />

  );
}

VideoStreamResetLogoDialog.displayName = 'VideoStreamResetLogoDialog';
VideoStreamResetLogoDialog.defaultProps = {
}

type VideoStreamResetLogoDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  value?: VideoStreamDto | undefined;
};

export default memo(VideoStreamResetLogoDialog);
