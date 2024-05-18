import React, { useMemo } from 'react';

import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import VisibleButton from '../buttons/VisibleButton';
import { ChannelGroupDto, UpdateChannelGroupRequest, UpdateChannelGroupsRequest } from '@lib/smAPI/smapiTypes';
import { UpdateChannelGroup, UpdateChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';

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
  const { selectedItems } = useSelectedItems<ChannelGroupDto>(id);
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
      toSend.ChannelGroupId = value.Id;
      toSend.ToggleVisibility = true;
      UpdateChannelGroup(toSend)
        .then(() => {
          setInfoMessage('Channel Group Toggle Visibility Successfully');
        })
        .catch((error) => {
          setInfoMessage(`Channel Group Toggle Visibility Error: ${error.message}`);
        });
    } else if (selectedItems) {
      const toSend = {} as UpdateChannelGroupsRequest;
      toSend.requests = selectedItems.map(
        (item) =>
          ({
            ChannelGroupId: item.Id,
            ToggleVisibility: true
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
      return value.IsReadOnly;
    }

    if (!selectedItems || selectedItems?.length === 0) {
      return true;
    }

    return selectedItems[0].IsReadOnly;
  }, [value, selectedItems]);

  const getTotalCount = useMemo(() => {
    if (selectAll) {
      return 100;
    }

    const count = selectedItems?.length ?? 0;
    // if (count === 1 && isFirstDisabled) {
    //   return 0;
    // }

    return count;
  }, [selectAll, selectedItems]);

  if (value) {
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
        // disabled={getTotalCount === 0}
        iconFilled={false}
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
