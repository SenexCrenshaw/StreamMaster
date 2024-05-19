import React, { useMemo } from 'react';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { ChannelGroupDto, DeleteChannelGroupRequest, DeleteChannelGroupsRequest } from '@lib/smAPI/smapiTypes';
import { DeleteChannelGroup, DeleteChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import SMDialog from '@components/sm/SMDialog';
import OKButton from '@components/buttons/OKButton';

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
      const toSend = {} as DeleteChannelGroupRequest;
      toSend.ChannelGroupId = value.Id;

      DeleteChannelGroup(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        });
    } else if (selectedItems) {
      const toSend = {} as DeleteChannelGroupsRequest;
      toSend.ChannelGroupIds = selectedItems.map((item) => item.Id);
      DeleteChannelGroups(toSend)
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

  const isDisabled = useMemo(() => {
    if (value) {
      return value.IsReadOnly;
    }
    return false;
  }, [value]);

  return (
    <SMDialog
      buttonDisabled={isDisabled}
      title="DELETE GROUP"
      iconFilled={value === undefined}
      onHide={() => ReturnToParent()}
      buttonClassName="icon-red"
      icon="pi-times"
      widthSize={2}
      info="General"
      tooltip="Delete Group"
      header={<OKButton onClick={onDeleteClick} tooltip="Delete Group" />}
    >
      <div className="flex justify-content-center w-full mb-2">{message}</div>
    </SMDialog>
  );
};

ChannelGroupDeleteDialog.displayName = 'ChannelGroupDeleteDialog';

export default React.memo(ChannelGroupDeleteDialog);
