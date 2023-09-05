
import { useCallback, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { useVideoStreamsSetVideoStreamsLogoFromEpgMutation, type VideoStreamsSetVideoStreamsLogoFromEpgApiArg } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import { Button } from "primereact/button";

const VideoStreamSetLogoFromEPGDialog = (props: VideoStreamSetLogoFromEPGDialogProps) => {

  const [videoStreamsSetVideoStreamsLogoFromEpgMutation] = useVideoStreamsSetVideoStreamsLogoFromEpgMutation();
  const ReturnToParent = useCallback(() => {
    props.onClose?.();
  }, [props]);


  const onChangeLogo = useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined) {
      ReturnToParent();

      return;
    }

    const toSend = {} as VideoStreamsSetVideoStreamsLogoFromEpgApiArg;

    toSend.ids = [props.value.id];

    await videoStreamsSetVideoStreamsLogoFromEpgMutation(toSend)
      .then(() => {
      }
      ).catch((error) => {
        console.log(error);
      });

  }, [ReturnToParent, props.value, videoStreamsSetVideoStreamsLogoFromEpgMutation]);

  return (
    <Button
      disabled={props.value === undefined || props.value.id === undefined}
      icon='pi pi-image'
      onClick={async () =>
        await onChangeLogo()
      }
      rounded
      size="small"
      text={props.iconFilled !== true}
      tooltip="Change Logo to EPG"
      tooltipOptions={getTopToolOptions}
    />
  );
}

VideoStreamSetLogoFromEPGDialog.displayName = 'VideoStreamSetLogoFromEPGDialog';
VideoStreamSetLogoFromEPGDialog.defaultProps = {
}

type VideoStreamSetLogoFromEPGDialogProps = {
  readonly iconFilled?: boolean | undefined;
  readonly onClose?: (() => void);
  readonly value?: VideoStreamDto | undefined;
};

export default memo(VideoStreamSetLogoFromEPGDialog);
