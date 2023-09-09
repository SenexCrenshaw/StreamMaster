
import React, { useMemo } from "react";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useChannelGroupsUpdateChannelGroupsMutation, type ChannelGroupDto, type UpdateChannelGroupRequest, type UpdateChannelGroupsRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import VisibleButton from "../buttons/VisibleButton";


type ChannelGroupVisibleDialogProps = {
  readonly id: string;
  readonly onClose?: (() => void);
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto[] | null;
};

const ChannelGroupVisibleDialog = ({ id, onClose, skipOverLayer, value }: ChannelGroupVisibleDialogProps) => {

  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);
  const [infoMessage, setInfoMessage] = React.useState('');

  const { selectAll } = useSelectAll(id);

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

    data.channelGroupRequests = selectedChannelGroups.map((item) => { return { channelGroupId: item.id, toggleVisibility: true } as UpdateChannelGroupRequest; });

    channelGroupsUpdateChannelGroupsMutation(data).then(() => {
      setInfoMessage('Channel Group Toggle Visibility Successfully');
    }).catch((e) => {
      setInfoMessage('Channel Group Toggle Visibility Error: ' + e.message);
    });

  }, [ReturnToParent, channelGroupsUpdateChannelGroupsMutation, selectedChannelGroups]);


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
        tooltip="Toggle2 Visibility"
      />
    </>
  );
}

ChannelGroupVisibleDialog.displayName = 'ChannelGroupVisibleDialog';


export default React.memo(ChannelGroupVisibleDialog);
