
import React from 'react';
import type * as StreamMasterApi from '../store/iptvApi';
import VideoStreamPanel from './VideoStreamPanel';
import { Button } from 'primereact/button';
import { getTopToolOptions } from '../common/common';
import InfoMessageOverLayDialog from './InfoMessageOverLayDialog';
import * as Hub from "../store/signlar_functions";

const VideoStreamAddDialog = (props: VideoStreamAddDialogProps) => {
  const [block, setBlock] = React.useState<boolean>(false);
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  };

  const onSave = async (e: StreamMasterApi.AddVideoStreamRequest) => {

    if (e === null || e === undefined) {
      return;
    }

    setBlock(true);
    const data = {} as StreamMasterApi.AddVideoStreamRequest

    data.tvg_name = e.tvg_name;


    if (e.tvg_group !== undefined) {
      data.tvg_group = e.tvg_group;
    }

    if (e.tvg_chno !== undefined && e.tvg_chno !== 0) {
      data.tvg_chno = e.tvg_chno;
    }

    if (e.tvg_ID !== undefined && e.tvg_ID !== '') {
      data.tvg_ID = e.tvg_ID;
    }

    if (e.tvg_logo !== undefined && e.tvg_logo !== '') {
      data.tvg_logo = e.tvg_logo;
    }

    if (e.url !== undefined && e.url !== '') {
      data.url = e.url;
    }

    // if (channelHandler !== null && channelHandler !== undefined) {
    //   // const channelHandlerInt = parseInt(channelHandler);
    //   data.iptvChannelHandler = channelHandler;
    // }

    await Hub.AddVideoStream(data)
      .then((returnData) => {
        if (returnData) {
          setInfoMessage('Add Stream Successful');
        } else {
          setInfoMessage('Add Stream No Changes');
        }
      }).catch((error) => {
        setInfoMessage('Add Stream Error: ' + error.message);
      }
      );
  };

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Add Video Stream"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={8}
        show={showOverlay}
      >

        <VideoStreamPanel
          group={props.group}
          onClose={() => ReturnToParent()}
          onSave={async (e) => await onSave(e)}
        />

      </InfoMessageOverLayDialog>

      <Button
        icon="pi pi-plus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="success"
        size="small"
        tooltip="Add Custom Stream"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
};

VideoStreamAddDialog.displayName = 'VideoStreamAddDialog';
VideoStreamAddDialog.defaultProps = {

};
type VideoStreamAddDialogProps = {
  group?: string | undefined;
  onClose?: (() => void);
};

export default React.memo(VideoStreamAddDialog);
