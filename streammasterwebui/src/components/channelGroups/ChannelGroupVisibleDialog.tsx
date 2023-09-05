
import React from "react";
import { type ChannelGroupDto, type UpdateChannelGroupRequest, type UpdateChannelGroupsRequest } from "../../store/iptvApi";
import { useChannelGroupsUpdateChannelGroupsMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import VisibleButton from "../buttons/VisibleButton";


type ChannelGroupVisibleDialogProps = {
  readonly onClose?: (() => void);
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto[] | null;
};

const ChannelGroupVisibleDialog = ({ onClose, skipOverLayer, value }: ChannelGroupVisibleDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);
  const [infoMessage, setInfoMessage] = React.useState('');

  const [channelGroupsUpdateChannelGroupsMutation] = useChannelGroupsUpdateChannelGroupsMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);

  React.useMemo(() => {

    if (value !== null && value !== undefined) {
      setSelectedChannelGroups(value);
    }

  }, [value]);

  const onVisibleClick = React.useCallback(async () => {
    setBlock(true);

    if (selectedChannelGroups.length === 0) {
      ReturnToParent();

      return;
    }

    const data = {} as UpdateChannelGroupsRequest;

    data.channelGroupRequests = selectedChannelGroups.map((item) => { return { channelGroupName: item.name, isHidden: !item.isHidden } as UpdateChannelGroupRequest; });

    channelGroupsUpdateChannelGroupsMutation(data).then(() => {
      setInfoMessage('Channel Group Set Visibilty Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Set Visibilty Error: ' + e.message);
    });

  }, [ReturnToParent, channelGroupsUpdateChannelGroupsMutation, selectedChannelGroups]);


  if (skipOverLayer === true) {
    return (

      <VisibleButton
        disabled={selectedChannelGroups.length === 0}
        iconFilled={false}
        onClick={async () => await onVisibleClick()}
        tooltip="Set Visibilty"
      />


    )
  }

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={`Toggle visibility for ${selectedChannelGroups.length < 2 ? selectedChannelGroups.length + ' Group ?' : selectedChannelGroups.length + ' Groups ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >

        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />

            <div className="flex col-12 mt-3 gap-2 justify-content-end">
              <VisibleButton label='Set Visibilty' onClick={async () => await onVisibleClick()} tooltip='Set Visibilty' />

            </div>
          </div>
        </div>

      </InfoMessageOverLayDialog>

      <VisibleButton
        disabled={selectedChannelGroups.length === 0}
        onClick={async () => {
          if (selectedChannelGroups.length > 1) {
            setShowOverlay(true);
          } else {
            await onVisibleClick();
          }
        }
        }
        tooltip="Set Visibilty"
      />

      {/* <Button
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
        text={iconFilled !== true}
        tooltip="Set Visibilty"
        tooltipOptions={getTopToolOptions}
      /> */}

    </>
  );
}

ChannelGroupVisibleDialog.displayName = 'ChannelGroupVisibleDialog';


export default React.memo(ChannelGroupVisibleDialog);
