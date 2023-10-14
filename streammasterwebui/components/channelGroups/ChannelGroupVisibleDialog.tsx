import { type ChannelGroupDto, type UpdateChannelGroupRequest, type UpdateChannelGroupsRequest } from '@lib/iptvApi';
import React, { useMemo } from 'react';

import { UpdateChannelGroup, UpdateChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsMutateAPI';

import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import VisibleButton from '../buttons/VisibleButton';

type ChannelGroupVisibleDialogProps = {
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto | undefined;
};

const ChannelGroupVisibleDialog = ({ id, onClose, skipOverLayer = false, value }: ChannelGroupVisibleDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const { selectSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');
  const { selectAll } = useSelectAll(id);

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);

  const onVisibleClick = React.useCallback(async () => {
    setBlock(true);

    if (!value && selectSelectedItems.length === 0) {
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
        .catch((e) => {
          setInfoMessage('Channel Group Toggle Visibility Error: ' + e.message);
        });
    } else if (selectSelectedItems) {
      const toSend = {} as UpdateChannelGroupsRequest;
      toSend.channelGroupRequests = selectSelectedItems.map((item) => {
        return {
          channelGroupId: item.id,
          toggleVisibility: true,
        } as UpdateChannelGroupRequest;
      });
      UpdateChannelGroups(toSend)
        .then(() => {
          setInfoMessage('Channel Group Toggle Visibility Successfully');
        })
        .catch((e) => {
          setInfoMessage('Channel Group Toggle Visibility Error: ' + e.message);
        });
    }
  }, [ReturnToParent, selectSelectedItems, value]);

  const isFirstDisabled = useMemo(() => {
    if (value) {
      return value.isReadOnly;
    }

    if (!selectSelectedItems || selectSelectedItems?.length === 0) {
      return true;
    }

    return selectSelectedItems[0].isReadOnly;
  }, [value, selectSelectedItems]);

  const getTotalCount = useMemo(() => {
    if (selectAll) {
      return 100;
    }

    let count = selectSelectedItems?.length ?? 0;
    if (count === 1 && isFirstDisabled) {
      return 0;
    }

    return count;
  }, [isFirstDisabled, selectAll, selectSelectedItems]);

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
          if (selectSelectedItems.length > 1) {
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
