
import { useState, useCallback, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
import { UpdateVideoStream } from "../../store/signlar_functions";
import { Button } from "primereact/button";

const VideoStreamSetLogoFromEPGDialog = (props: VideoStreamSetLogoFromEPGDialogProps) => {

  const [epgLogoUrl, setEpgLogoUrl] = useState<string | undefined>(undefined);

  const ReturnToParent = useCallback(() => {
    setEpgLogoUrl(undefined);
    props.onClose?.();
  }, [props]);


  const onChangeLogo = useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined || epgLogoUrl === undefined || epgLogoUrl === undefined) {
      ReturnToParent();
      return;
    }

    const toSend = {} as UpdateVideoStreamRequest;
    toSend.id = props.value?.id;
    toSend.tvg_logo = epgLogoUrl;

    await UpdateVideoStream(toSend)
      .then(() => {
      }
      ).catch((error) => {
        console.log(error);
      });

  }, [ReturnToParent, epgLogoUrl, props.value]);

  return (
    <Button
      disabled={!epgLogoUrl || props.value?.user_Tvg_logo === epgLogoUrl}
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
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  value?: VideoStreamDto | undefined;
};

export default memo(VideoStreamSetLogoFromEPGDialog);
