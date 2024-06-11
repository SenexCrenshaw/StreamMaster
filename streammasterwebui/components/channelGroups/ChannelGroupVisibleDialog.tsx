import { SMDialogRef } from '@components/sm/SMDialog';
import { SMPopUp } from '@components/sm/SMPopUp';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { UpdateChannelGroup, UpdateChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { ChannelGroupDto, UpdateChannelGroupRequest, UpdateChannelGroupsRequest } from '@lib/smAPI/smapiTypes';
import React, { useMemo } from 'react';
import VisibleButton from '../buttons/VisibleButton';

interface ChannelGroupVisibleDialogProperties {
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto | undefined;
}

const ChannelGroupVisibleDialog = ({ id, onClose, skipOverLayer = false, value }: ChannelGroupVisibleDialogProperties) => {
  const { selectedItems } = useSelectedItems<ChannelGroupDto>(id);
  const { selectAll } = useSelectAll(id);
  const dialogRef = React.useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    onClose?.();
  }, [onClose]);

  const onVisibleClick = React.useCallback(async () => {
    if (!value && selectedItems.length === 0) {
      ReturnToParent();
      return;
    }

    if (value) {
      const toSend = {} as UpdateChannelGroupRequest;
      toSend.ChannelGroupId = value.Id;
      toSend.ToggleVisibility = true;
      UpdateChannelGroup(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          dialogRef.current?.hide();
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
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          dialogRef.current?.hide();
        });
    }
  }, [ReturnToParent, selectedItems, value]);

  const message = useMemo(() => {
    if (value) {
      return `Toggle Visibility for '${value.Name}'?`;
    }

    if (selectAll) {
      return `Toggle Visibility for Groups?`;
    }

    return `Toggle Visibility for Groups (${selectedItems.length})?`;
  }, [selectAll, selectedItems.length, value]);

  if (value) {
    return <VisibleButton iconFilled={false} onClick={async () => await onVisibleClick()} />;
  }

  return (
    <SMPopUp
      buttonClassName="icon-blue"
      buttonDisabled={selectedItems.length === 0 && !selectAll}
      icon="pi-eye-slash"
      iconFilled
      rememberKey={'ChannelGroupVisibility'}
      title="TOGGLE VISIBILITY"
      onOkClick={async () => await onVisibleClick()}
    >
      {message}
    </SMPopUp>
  );
};

ChannelGroupVisibleDialog.displayName = 'ChannelGroupVisibleDialog';

export default React.memo(ChannelGroupVisibleDialog);
