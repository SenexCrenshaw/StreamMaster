import { SMPopUp } from '@components/sm/SMPopUp';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { DeleteAllChannelGroupsFromParameters, DeleteChannelGroup, DeleteChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { ChannelGroupDto, DeleteAllChannelGroupsFromParametersRequest, DeleteChannelGroupRequest, DeleteChannelGroupsRequest } from '@lib/smAPI/smapiTypes';
import React, { useMemo } from 'react';

interface ChannelGroupDeleteDialogProperties {
  readonly id: string;
  readonly onClose?: () => void;
  readonly value?: ChannelGroupDto | undefined;
}

const ChannelGroupDeleteDialog = ({ id, onClose, value }: ChannelGroupDeleteDialogProperties) => {
  const { selectedItems, setSelectedItems } = useSelectedItems<ChannelGroupDto>(id);
  const { selectAll, setSelectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

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
      return (
        <div>
          Delete <div className="text-container">{value.Name}</div> Channel Group?
        </div>
      );
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
    <SMPopUp
      buttonDisabled={isDisabled}
      iconFilled={value === undefined}
      rememberKey={'DeleteChannelGroupDialog'}
      title="Delete"
      onOkClick={() => onDeleteClick()}
      icon="pi-times"
    >
      {message}
    </SMPopUp>
  );
};

ChannelGroupDeleteDialog.displayName = 'ChannelGroupDeleteDialog';

export default React.memo(ChannelGroupDeleteDialog);
