import React, { useMemo } from 'react';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { ChannelGroupDto, DeleteAllChannelGroupsFromParametersRequest, DeleteChannelGroupRequest, DeleteChannelGroupsRequest } from '@lib/smAPI/smapiTypes';
import { DeleteAllChannelGroupsFromParameters, DeleteChannelGroup, DeleteChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import OKButton from '@components/buttons/OKButton';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';

interface ChannelGroupDeleteDialogProperties {
  readonly id: string;
  readonly onClose?: () => void;
  readonly value?: ChannelGroupDto | undefined;
}

const ChannelGroupDeleteDialog = ({ id, onClose, value }: ChannelGroupDeleteDialogProperties) => {
  const { selectedItems, setSelectedItems } = useSelectedItems<ChannelGroupDto>(id);
  const { selectAll, setSelectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);
  const dialogRef = React.useRef<SMDialogRef>(null);

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
      return;
    }

    if (selectAll) {
      if (!queryFilter) {
        ReturnToParent();
        return;
      }

      const request = {} as DeleteAllChannelGroupsFromParametersRequest;
      request.Parameters = queryFilter;

      await DeleteAllChannelGroupsFromParameters(request)
        .then(() => {
          setSelectedItems([]);
          setSelectAll(false);
        })
        .catch((error) => {
          console.error(error);
          throw error;
        })
        .finally(() => {
          // dialogRef.current?.close();
        });

      return;
    }

    if (selectedItems) {
      const toSend = {} as DeleteChannelGroupsRequest;
      toSend.ChannelGroupIds = selectedItems.map((item) => item.Id);
      DeleteChannelGroups(toSend)
        .then(() => {
          setSelectedItems([]);
          setSelectAll(false);
        })
        .catch((error) => {
          console.error(error);
        });
    }
  }, [ReturnToParent, queryFilter, selectAll, selectedItems, setSelectAll, setSelectedItems, value]);

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

    if (selectAll === true) {
      return false;
    }

    if (selectedItems.length === 0) {
      return true;
    }

    if (selectedItems.every((predicate) => predicate.IsReadOnly)) {
      return true;
    }

    return false;
  }, [selectAll, selectedItems, value]);

  return (
    <SMDialog
      ref={dialogRef}
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
