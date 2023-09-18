
import { Button } from "primereact/button";
import { memo, useCallback } from "react";
import { getTopToolOptions } from "../../common/common";
import { SetVideoStreamsLogoFromEpg } from "../../smAPI/VideoStreams/VideoStreamsMutateAPI";
import { type VideoStreamDto, type VideoStreamsSetVideoStreamsLogoFromEpgApiArg } from "../../store/iptvApi";

const VideoStreamSetLogoFromEPGDialog = (props: VideoStreamSetLogoFromEPGDialogProps) => {

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

    await SetVideoStreamsLogoFromEpg(toSend)
      .then(() => {
      }
      ).catch((error) => {
        console.log(error);
      });

  }, [ReturnToParent, props.value]);

  return (
    <Button
      disabled={props.value === undefined || props.value.id === undefined || props.value.isUserCreated === true}
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
