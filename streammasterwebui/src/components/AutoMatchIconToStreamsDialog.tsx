/* eslint-disable react/no-unused-prop-types */
import { Button } from "primereact/button";

import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';
import { AutoMatchIconToStreams } from "../store/signlar_functions";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";
import { getTopToolOptions } from "../common/common";
import { AutoMatchIcon } from "../common/icons";

const AutoMatchIconToStreamsDialog = (props: AutoMatchIconToStreamsDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [block, setBlock] = React.useState<boolean>(false);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  };


  const onAutoMatch = React.useCallback(async () => {
    setBlock(true);

    const data = {} as StreamMasterApi.AutoMatchIconToStreamsRequest;
    data.ids = props.ids.map((item: StreamMasterApi.ChannelNumberPair) => item.id);

    AutoMatchIconToStreams(data)
      .then(() => {
        setInfoMessage('Auto Set Channels Successful');
      }
      ).catch((e) => {
        setInfoMessage('Auto Set Channels Error: ' + e.message);
      });


  }, [props]);


  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header="Auto Set Channel Numbers"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className='flex flex-column mt-2 col-6'>
            Auto set (${props.ids.length}) channel logos ?
            <span className="scalein animation-duration-500 animation-iteration-2 text-bold text-red-500 font-italic mt-2">
              This will auto match
            </span>
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
              onClick={async () => await onAutoMatch()}
              rounded
              severity="success"
              size="small"
            />
          </div>
        </div>
      </InfoMessageOverLayDialog>


      <Button
        disabled={props.ids === undefined || props.ids.length === 0}
        icon={<AutoMatchIcon />}
        onClick={() => setShowOverlay(true)}
        rounded
        size="small"
        tooltip={`Auto Match (${props.ids.length}) Logos`}
        tooltipOptions={getTopToolOptions}
      />

    </>
  )
}

AutoMatchIconToStreamsDialog.displayName = 'Auto Match Logos';
AutoMatchIconToStreamsDialog.defaultProps = {
};

export type AutoMatchIconToStreamsDialogProps = {
  ids: StreamMasterApi.ChannelNumberPair[];
  onChange?: ((value: StreamMasterApi.ChannelNumberPair[]) => void) | null;
};

export default React.memo(AutoMatchIconToStreamsDialog);
