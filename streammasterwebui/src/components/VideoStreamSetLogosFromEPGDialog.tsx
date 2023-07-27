import { Button } from "primereact/button";

import React from "react";
// import StreamMasterSetting from "../store/signlar/StreamMasterSetting";
import type * as StreamMasterApi from '../store/iptvApi';
import { SetVideoStreamsLogoToEPG } from "../store/signlar_functions";
import { getTopToolOptions } from "../common/common";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const VideoStreamSetLogosFromEPGDialog = (props: VideoStreamSetLogosFromEPGDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [block, setBlock] = React.useState<boolean>(false);


  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  };

  const onSetLogoSave = React.useCallback(async () => {
    setBlock(true);


    const ids = [...new Set(props.values.map((item: StreamMasterApi.VideoStreamDto) => item.id))] as string[];

    const toSend = {} as StreamMasterApi.SetVideoStreamsLogoToEpgRequest;
    const max = 500;

    let count = 0;

    const promises = [];

    while (count < ids.length) {
      if (count + max < ids.length) {
        toSend.ids = ids.slice(count, count + max);
      } else {
        toSend.ids = ids.slice(count, ids.length);
      }

      count += max;
      promises.push(
        SetVideoStreamsLogoToEPG(toSend)
          .then(() => {
          }).catch(() => { })
      );

    }

    const p = Promise.all(promises);

    await p.then(() => {
      setInfoMessage('Auto Set Channels Successful');
      // props.onChange?.(ret);
    }).catch((error) => {
      setInfoMessage('Auto Set Channels Error: ' + error.message);
    });


  }, [props]);


  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header="Match Logos"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className='flex flex-column mt-2 col-6'>
            {`Match (${props.values.length}) video stream logo${props.values.length > 1 ? 's' : ''} to ${props.values.length > 1 ? 'their' : 'its'} EPG logo${props.values.length > 1 ? 's' : ''}?'`}
          </div>

          <div className="flex col-12 gap-2 mt-4 justify-content-center ">
            <Button
              icon="pi pi-times "
              label="Cancel"
              onClick={() => ReturnToParent()}
              rounded
              severity="warning"
              size="small"
            />
            <Button
              icon="pi pi-check"
              label="Set & Save"
              onClick={async () => await onSetLogoSave()}
              rounded
              severity="success"
              size="small"
            />
          </div>
        </div>
      </InfoMessageOverLayDialog>


      <Button
        disabled={props.values === undefined || props.values.length === 0}
        icon="pi pi-image"
        onClick={() => setShowOverlay(true)}
        rounded
        size="small"
        tooltip={`Set Logo from EPG for (${props.values.length}) Streams`}
        tooltipOptions={getTopToolOptions}
      />

    </>
  )
}

VideoStreamSetLogosFromEPGDialog.displayName = 'Auto Set Channel Numbers';
VideoStreamSetLogosFromEPGDialog.defaultProps = {
};

export type VideoStreamSetLogosFromEPGDialogProps = {
  // onChange?: ((value: StreamMasterApi.ChannelNumberPair[]) => void) | null;
  values: StreamMasterApi.VideoStreamDto[];
};

export default React.memo(VideoStreamSetLogosFromEPGDialog);
