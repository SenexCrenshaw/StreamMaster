
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";

import { getTopToolOptions } from "../common/common";
import InfoMessageOverLayDialog from "./InfoMessageOverLayDialog";

const ChannelGroupVisibleDialog = (props: ChannelGroupVisibleDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);
  const [infoMessage, setInfoMessage] = React.useState('');

  const [channelGroupsSetChannelGroupsVisible] = StreamMasterApi.useChannelGroupsSetChannelGroupsVisibleMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  }, [props]);

  React.useMemo(() => {

    if (props.value !== null && props.value !== undefined) {
      setSelectedChannelGroups(props.value);
    }

  }, [props.value]);

  const onVisibleClick = React.useCallback(async () => {
    setBlock(true);
    if (selectedChannelGroups.length === 0) {
      ReturnToParent();
      return;
    }

    const data = {} as StreamMasterApi.SetChannelGroupsVisibleRequest;
    data.requests = selectedChannelGroups.map((item) => { return { groupName: item.name, isHidden: !item.isHidden }; });
    channelGroupsSetChannelGroupsVisible(data).then(() => {
      setInfoMessage('Channel Group Set Visibilty Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Set Visibilty Error: ' + e.message);
    });

  }, [ReturnToParent, channelGroupsSetChannelGroupsVisible, selectedChannelGroups]);


  if (props.skipOverLayer === true) {
    return (

      <Button
        disabled={selectedChannelGroups.length === 0}
        icon="pi pi-power-off"
        onClick={async () => await onVisibleClick()}
        rounded
        severity="info"
        size="small"
        text={props.iconFilled !== true}
        tooltip="Set Visibilty"
        tooltipOptions={getTopToolOptions}
      />

    )
  }

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header={`Toggle visibility for ${selectedChannelGroups.length < 2 ? selectedChannelGroups.length + ' Group ?' : selectedChannelGroups.length + ' Groups ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >

        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />

            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <Button
                icon="pi pi-times "
                label="Cancel"
                onClick={(() => ReturnToParent())}
                rounded
                severity="warning"
              />
              <Button
                icon="pi pi-check"
                label="Change"
                onClick={async () => await onVisibleClick()}
                rounded
                severity="success"
              />
            </div>
          </div>
        </div>

      </InfoMessageOverLayDialog>

      <Button
        disabled={selectedChannelGroups.length === 0}
        icon="pi pi-power-off"
        onClick={async () => {
          if (selectedChannelGroups.length > 1) {
            setShowOverlay(true);
          } else {
            await onVisibleClick();
          }
        }
        }
        rounded
        severity="info"
        size="small"
        text={props.iconFilled !== true}
        tooltip="Set Visibilty"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

ChannelGroupVisibleDialog.displayName = 'ChannelGroupVisibleDialog';
ChannelGroupVisibleDialog.defaultProps = {
  iconFilled: true,
  value: null,
};

type ChannelGroupVisibleDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  skipOverLayer?: boolean | undefined;
  value?: StreamMasterApi.ChannelGroupDto[] | null;
};

export default React.memo(ChannelGroupVisibleDialog);
