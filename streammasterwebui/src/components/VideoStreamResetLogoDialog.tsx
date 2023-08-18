import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Button } from "primereact/button";

import { getTopToolOptions } from "../common/common";
import { ResetLogoIcon } from "../common/icons";


const VideoStreamResetLogoDialog = (props: VideoStreamResetLogoDialogProps) => {

  const ReturnToParent = React.useCallback(() => {
    props.onClose?.();
  }, [props]);

  const onResetLogo = React.useCallback(async () => {

    if (props.value === undefined || props.value.id === undefined) {
      ReturnToParent();
      return;
    }

    const toSend = {} as StreamMasterApi.UpdateVideoStreamRequest;
    toSend.id = props.value?.id;
    toSend.tvg_logo = props.value.tvg_logo;

    await Hub.UpdateVideoStream(toSend)
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
  value?: StreamMasterApi.VideoStreamDto | undefined;
};

export default React.memo(VideoStreamResetLogoDialog);
