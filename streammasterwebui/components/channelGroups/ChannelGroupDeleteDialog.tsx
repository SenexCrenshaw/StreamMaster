import {
  useChannelGroupsDeleteAllChannelGroupsFromParametersMutation,
  useChannelGroupsDeleteChannelGroupMutation,
  type ChannelGroupDto,
  type ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg,
  type DeleteChannelGroupRequest,
} from '@/lib/iptvApi';
import { memo, useCallback, useMemo, useState } from 'react';

import { useQueryFilter } from '@/lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@/lib/redux/slices/useSelectAll';
import { useSelectedItems } from '@/lib/redux/slices/useSelectedItemsSlice';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import XButton from '../buttons/XButton';

type ChannelGroupDeleteDialogProps = {
  readonly iconFilled?: boolean | undefined;
  readonly id: string;
  readonly onDelete?: (results: number[] | undefined) => void;
  readonly onHide?: () => void;
  readonly value?: ChannelGroupDto | undefined;
};

const ChannelGroupDeleteDialog = ({ iconFilled, id, onDelete, onHide, value }: ChannelGroupDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);

  const [infoMessage, setInfoMessage] = useState('');

  const { selectAll, setSelectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');

  const [channelGroupsDeleteChannelGroupMutation] = useChannelGroupsDeleteChannelGroupMutation();
  const [channelGroupsDeleteAllChannelGroupsFromParametersMutation] = useChannelGroupsDeleteAllChannelGroupsFromParametersMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onHide?.();
  }, [onHide]);

  const deleteGroup = useCallback(async () => {
    setBlock(true);

    if (value?.id) {
      const toSend = {} as DeleteChannelGroupRequest;

      toSend.channelGroupId = value.id;

      await channelGroupsDeleteChannelGroupMutation(toSend)
        .then(() => {
          setInfoMessage('Deleted Successfully');
        })
        .catch((error) => {
          setInfoMessage('Delete Error: ' + error.message);
        });

      return;
    }

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();

        return;
      }

      const toSendAll = {} as ChannelGroupsDeleteAllChannelGroupsFromParametersApiArg;

      toSendAll.parameters = queryFilter;

      await channelGroupsDeleteAllChannelGroupsFromParametersMutation(toSendAll)
        .then(() => {
          setInfoMessage('Deleted Successfully');
          setSelectSelectedItems([]);
          setSelectAll(false);
        })
        .catch((error) => {
          setInfoMessage('Delete Error: ' + error.message);
          setSelectSelectedItems([]);
          setSelectAll(false);
        });

      return;
    }

    if ((selectSelectedItems || []).length === 0) {
      ReturnToParent();

      return;
    }

    const promises = [];
    const groupIds = [] as number[];

    for (const group of selectSelectedItems.filter((a) => a.id !== undefined && !a.isReadOnly)) {
      if (group.id === undefined || group.id == null) {
        continue;
      }

      const data = {} as DeleteChannelGroupRequest;

      data.channelGroupId = group.id;
      groupIds.push(group.id);
      promises.push(
        channelGroupsDeleteChannelGroupMutation(data)
          .then(() => {})
          .catch(() => {}),
      );
    }

    const p = Promise.all(promises);

    await p
      .then(() => {
        setInfoMessage('Channel Group Delete Successful');
        onDelete?.(groupIds);
      })
      .catch((error) => {
        setInfoMessage('Channel Group Delete Error: ' + error.message);
        onDelete?.(undefined);
      });

    setSelectSelectedItems(selectSelectedItems.filter((a) => !groupIds.includes(a.id ?? 0)));
    setSelectAll(false);
  }, [
    value,
    selectAll,
    selectSelectedItems,
    setSelectAll,
    channelGroupsDeleteChannelGroupMutation,
    queryFilter,
    channelGroupsDeleteAllChannelGroupsFromParametersMutation,
    ReturnToParent,
    setSelectSelectedItems,
    onDelete,
  ]);

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
    if (selectAll || !value?.isReadOnly) {
      return 1;
    }

    let count = selectSelectedItems?.length ?? 0;
    if (count === 1 && isFirstDisabled) {
      return 0;
    }

    return count;
  }, [isFirstDisabled, selectAll, selectSelectedItems, value]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Delete Group?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full mb-2">
          <XButton
            disabled={getTotalCount === 0 && !selectAll}
            label="Delete Groups"
            onClick={async () => await deleteGroup()}
            tooltip="Delete User Created Groups"
          />
        </div>
      </InfoMessageOverLayDialog>

      <XButton disabled={getTotalCount === 0} iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip="Delete User Created Group" />
    </>
  );
};

ChannelGroupDeleteDialog.displayName = 'ChannelGroupDeleteDialog';
export default memo(ChannelGroupDeleteDialog);
