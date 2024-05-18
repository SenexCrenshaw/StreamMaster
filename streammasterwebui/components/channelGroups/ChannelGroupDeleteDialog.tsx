import React, { useMemo } from 'react';

import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import VisibleButton from '../buttons/VisibleButton';
import { ChannelGroupDto, UpdateChannelGroupRequest, UpdateChannelGroupsRequest } from '@lib/smAPI/smapiTypes';
import { UpdateChannelGroup, UpdateChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import SMButton from '@components/sm/SMButton';
import StringEditor from '@components/inputs/StringEditor';
import SMDialog from '@components/sm/SMDialog';

interface ChannelGroupDeleteDialogProperties {
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean | undefined;
  readonly value?: ChannelGroupDto | undefined;
}

const ChannelGroupDeleteDialog = ({ id, onClose, skipOverLayer = false, value }: ChannelGroupDeleteDialogProperties) => {
  const { selectedItems } = useSelectedItems<ChannelGroupDto>(id);
  const { selectAll } = useSelectAll(id);

  const ReturnToParent = React.useCallback(() => {
    onClose?.();
  }, [onClose]);

  const onDeleteClick = React.useCallback(async () => {
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
        });
    }
  }, [ReturnToParent, selectedItems, value]);

  const message = useMemo(() => {
    if (value) {
      return `Delete '${value.Name}' Channel Group?`;
    }

    if (selectAll) {
      return `Delete All Selected Channel Groups?`;
    }

    return `Delete Selected Channel Groups (${selectedItems.length})?`;
  }, [selectAll, selectedItems.length, value]);

  return (
    <SMDialog
      title="DELETE GROUP"
      iconFilled={value === undefined}
      onHide={() => ReturnToParent()}
      buttonClassName="icon-red"
      icon="pi-times"
      widthSize={2}
      info="General"
      tooltip="Delete Group"
      header={<div>a</div>}
    >
      <div className="flex justify-content-center w-full mb-2">{message}</div>
    </SMDialog>
  );
};

ChannelGroupDeleteDialog.displayName = 'ChannelGroupDeleteDialog';

export default React.memo(ChannelGroupDeleteDialog);
