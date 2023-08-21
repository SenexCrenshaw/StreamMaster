
import { useCallback, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { ResetLogoIcon } from "../../common/icons";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
import { UpdateVideoStream } from "../../store/signlar_functions";
import { Button } from "primereact/button";

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
    <Button
      disabled={props.value?.tvg_logo === props.value?.user_Tvg_logo}
      icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
      onClick={async () =>
        await onResetLogo()
      }
      rounded
      size="small"
      text={props.iconFilled !== true}
      tooltip="Reset Logo From File"
      tooltipOptions={getTopToolOptions}
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
