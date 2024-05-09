import React, { useMemo } from 'react';

import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import VisibleButton from '../buttons/VisibleButton';

interface ChannelGroupVisibleDialogProperties {
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto | undefined;
}

const ChannelGroupVisibleDialog = ({ id, onClose, skipOverLayer = false, value }: ChannelGroupVisibleDialogProperties) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const { selectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');
  const { selectAll } = useSelectAll(id);

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);

  const onVisibleClick = React.useCallback(async () => {
    setBlock(true);

    if (!value && selectedItems.length === 0) {
      ReturnToParent();
      return;
    }

    if (value) {
      const toSend = {} as UpdateChannelGroupRequest;
      toSend.channelGroupId = value.id;
      toSend.toggleVisibility = true;
      UpdateChannelGroup(toSend)
        .then(() => {
          setInfoMessage('Channel Group Toggle Visibility Successfully');
        })
        .catch((error) => {
          setInfoMessage(`Channel Group Toggle Visibility Error: ${error.message}`);
        });
    } else if (selectedItems) {
      const toSend = {} as UpdateChannelGroupsRequest;
      toSend.channelGroupRequests = selectedItems.map(
        (item) =>
          ({
            channelGroupId: item.id,
            toggleVisibility: true
          } as UpdateChannelGroupRequest)
      );
      UpdateChannelGroups(toSend)
        .then(() => {
          setInfoMessage('Channel Group Toggle Visibility Successfully');
        })
        .catch((error) => {
          setInfoMessage(`Channel Group Toggle Visibility Error: ${error.message}`);
        });
    }
  }, [ReturnToParent, selectedItems, value]);

  const isFirstDisabled = useMemo(() => {
    if (value) {
      return value.isReadOnly;
    }

    if (!selectedItems || selectedItems?.length === 0) {
      return true;
    }

    return selectedItems[0].isReadOnly;
  }, [value, selectedItems]);

  const getTotalCount = useMemo(() => {
    if (selectAll) {
      return 100;
    }

    const count = selectedItems?.length ?? 0;
    if (count === 1 && isFirstDisabled) {
      return 0;
    }

    return count;
  }, [isFirstDisabled, selectAll, selectedItems]);

  if (skipOverLayer === true) {
    return <VisibleButton iconFilled={false} onClick={async () => await onVisibleClick()} />;
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Toggle Visibility?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full mb-2">
          <VisibleButton label="Toggle Visibility" onClick={async () => await onVisibleClick()} />
        </div>
      </InfoMessageOverLayDialog>
      <VisibleButton
        disabled={getTotalCount === 0}
        onClick={async () => {
          if (selectedItems.length > 1) {
            setShowOverlay(true);
          } else {
            await onVisibleClick();
          }
        }}
      />
    </>
  );
};

ChannelGroupVisibleDialog.displayName = 'ChannelGroupVisibleDialog';

export default React.memo(ChannelGroupVisibleDialog);
