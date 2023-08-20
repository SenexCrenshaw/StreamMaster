import React from "react";
import * as Hub from '../store/signlar_functions';
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../store/iptvApi";


const VideoStreamSetLogoFromEPGDialog = (props: VideoStreamSetLogoFromEPGDialogProps) => {

  const [epgLogoUrl, setEpgLogoUrl] = React.useState<string | undefined>(undefined);

  const ReturnToParent = React.useCallback(() => {
    setEpgLogoUrl(undefined);
    props.onClose?.();
  }, [props]);


  const onChangeLogo = React.useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined || epgLogoUrl === undefined || epgLogoUrl === undefined) {
      ReturnToParent();
      return;
    }

    const toSend = {} as UpdateVideoStreamRequest;
    toSend.id = props.value?.id;
    toSend.tvg_logo = epgLogoUrl;

    await Hub.UpdateVideoStream(toSend)
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

export default React.memo(VideoStreamSetLogoFromEPGDialog);
