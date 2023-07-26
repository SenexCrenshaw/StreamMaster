/* eslint-disable @typescript-eslint/no-unused-vars */

import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import VideoStreamPanel from "./VideoStreamPanel";

const VideoStreamSetIconFromEPGDialog = (props: VideoStreamSetIconFromEPGDialogProps) => {
  const channelLogos = StreamMasterApi.useVideoStreamsGetChannelLogoDtosQuery();
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  // const [videoStream, setVideoStream] = React.useState<StreamMasterApi.VideoStreamDto | undefined>(undefined);

  // React.useEffect(() => {
  //   setVideoStream(props.value);

  // }, [props.value]);

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    // setVideoStream(undefined);
    props.onClose?.();
  }, [props]);

  const onEdit = React.useCallback(async () => {
    setBlock(true);
    if (props.value === null || props.value === undefined || !channelLogos || channelLogos.data === null || channelLogos.data === undefined || channelLogos.data.length === 0) {
      ReturnToParent();
      return;
    }

    if (props.value.user_Tvg_logo === null || props.value.user_Tvg_logo === undefined || props.value.user_Tvg_logo === '') {
      ReturnToParent();
      return;
    }

    const index = channelLogos.data.findIndex((x) => x.logoUrl === props.value?.user_Tvg_logo);
    if (index === -1) {
      ReturnToParent();
      return;
    }

    const url = channelLogos.data[index].logoUrl;
    console.log('url: ' + url);

    // Hub.UpdateVideoStream(data)
    //   .then((resultData) => {
    //     if (resultData) {
    //       setInfoMessage('Set Stream Edited Successfully');

    //     } else {
    //       setInfoMessage('Set Stream Edited No Change');
    //     }
    //   }
    //   ).catch((error) => {
    //     setInfoMessage('Set Stream Edited Error: ' + error.message);
    //   });

  }, [ReturnToParent, channelLogos, props.value]);


  return (
    <Button
      // disabled={!videoStream}
      icon='pi pi-image'
      onClick={async () =>
        await onEdit()
      }
      rounded
      size="small"
      text={props.iconFilled !== true}
      tooltip="Edit Stream"
      tooltipOptions={getTopToolOptions}
    />
  );
}

VideoStreamSetIconFromEPGDialog.displayName = 'VideoStreamSetIconFromEPGDialog';
VideoStreamSetIconFromEPGDialog.defaultProps = {
}

type VideoStreamSetIconFromEPGDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  value?: StreamMasterApi.VideoStreamDto | undefined;
};

export default React.memo(VideoStreamSetIconFromEPGDialog);
