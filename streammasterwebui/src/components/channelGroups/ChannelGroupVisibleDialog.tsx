import React, { useMemo } from "react";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useSelectedChannelGroups } from "../../app/slices/useSelectedChannelGroups";
import { UpdateChannelGroup, UpdateChannelGroups } from "../../smAPI/ChannelGroups/ChannelGroupsMutateAPI";
import { type ChannelGroupDto, type UpdateChannelGroupRequest, type UpdateChannelGroupsRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import VisibleButton from "../buttons/VisibleButton";


type ChannelGroupVisibleDialogProps = {
  readonly cgid: string;
  readonly id: string;
  readonly onClose?: (() => void);
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto | undefined;
};

const ChannelGroupVisibleDialog = ({ id, cgid, onClose, skipOverLayer = false, value }: ChannelGroupVisibleDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);

  const [infoMessage, setInfoMessage] = React.useState('');

  const { selectedChannelGroups } = useSelectedChannelGroups(cgid);
  const { selectAll } = useSelectAll(id);

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);


  const onVisibleClick = React.useCallback(async () => {
    setBlock(true);

    if (selectedChannelGroups.length === 0) {
      ReturnToParent();

      return;
    }

    if (value) {
      const toSend = {} as UpdateChannelGroupRequest;
      toSend.channelGroupId = value.id;
      toSend.toggleVisibility = true;
      UpdateChannelGroup(toSend).then(() => {
        setInfoMessage('Channel Group Toggle Visibility Successfully');
      }).catch((e) => {
        setInfoMessage('Channel Group Toggle Visibility Error: ' + e.message);
      });
    } else if (selectedChannelGroups) {
      const toSend = {} as UpdateChannelGroupsRequest;
      toSend.channelGroupRequests = selectedChannelGroups.map((item) => { return { channelGroupId: item.id, toggleVisibility: true } as UpdateChannelGroupRequest; });
      UpdateChannelGroups(toSend).then(() => {
        setInfoMessage('Channel Group Toggle Visibility Successfully');
      }).catch((e) => {
        setInfoMessage('Channel Group Toggle Visibility Error: ' + e.message);
      });
    }

  }, [ReturnToParent, selectedChannelGroups, value]);


  const getTotalCount = useMemo(() => {

    if (selectAll) {
      return 100;
    }

    return selectedChannelGroups?.length ?? 0;

  }, [selectAll, selectedChannelGroups?.length]);


  if (skipOverLayer === true) {
    return (
      <VisibleButton
        disabled={getTotalCount === 0}
        iconFilled={false}
        onClick={async () => await onVisibleClick()}
        tooltip="Toggle Visibility"
      />
    )
  }

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header='Toggle Visibility?'
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >

        <div className="flex justify-content-center w-full">
          <VisibleButton label='Toggle Visibility' onClick={async () => await onVisibleClick()} tooltip='Toggle2 Visibility' />
        </div>

      </InfoMessageOverLayDialog>

      <VisibleButton
        disabled={getTotalCount === 0}
        onClick={async () => {
          if (selectedChannelGroups.length > 1) {
            setShowOverlay(true);
          } else {
            await onVisibleClick();
          }
        }
        }
        tooltip="Toggle Visibility"
      />
    </>
  );
}

ChannelGroupVisibleDialog.displayName = 'ChannelGroupVisibleDialog';


export default React.memo(ChannelGroupVisibleDialog);
