import { memo, useCallback } from "react";
import { UpdateVideoStream } from "../../hooks/streammasterSignalrHooks";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
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
      disabled={!props.value || props.value.tvg_logo === props.value.user_Tvg_logo}
      iconFilled={props.iconFilled}
      onClick={async () =>
        await onResetLogo()
      }
      tooltip="Reset Logo"
    />

  );
}

VideoStreamResetLogoDialog.displayName = 'VideoStreamResetLogoDialog';
VideoStreamResetLogoDialog.defaultProps = {
}

type VideoStreamResetLogoDialogProps = {
  readonly iconFilled?: boolean | undefined;
  readonly onClose?: (() => void);
  readonly value?: VideoStreamDto | undefined;
};

export default memo(VideoStreamResetLogoDialog);
