import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';

import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { DeleteSMChannels, DeleteSMChannelsFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannelsFromParametersRequest, DeleteSMChannelsRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useMemo } from 'react';

interface DeleteSMChannelsDialogProperties {
  readonly onClose?: () => void;
  readonly id: string;
  readonly selectedItemsKey: string;
}

const DeleteSMChannelsDialog = ({ id, onClose, selectedItemsKey }: DeleteSMChannelsDialogProperties) => {
  const dialogRef = React.useRef<SMDialogRef>(null);
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<SMStreamDto>(selectedItemsKey);

  const { selectAll, setSelectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const getTotalCount = useMemo(() => selectSelectedItems?.length ?? 0, [selectSelectedItems]);
  const ReturnToParent = useCallback(() => {
    onClose?.();
  }, [onClose]);

  const onOkClick = useCallback(async () => {
    if (selectSelectedItems === undefined) {
      ReturnToParent();
      return;
    }

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();
        return;
      }

      const request = {} as DeleteSMChannelsFromParametersRequest;
      request.Parameters = queryFilter;

      await DeleteSMChannelsFromParameters(request)
        .then(() => {
          setSelectSelectedItems([]);
          setSelectAll(false);
        })
        .catch((error) => {
          console.error(error);
          throw error;
        })
        .finally(() => {
          dialogRef.current?.close();
        });

      return;
    }

    if (selectSelectedItems.length === 0) {
      ReturnToParent();

      return;
    }

    const request = {} as DeleteSMChannelsRequest;

    const ids: number[] = selectSelectedItems.map((item) => Number(item.Id));
    request.SMChannelIds = ids;

    await DeleteSMChannels(request)
      .then(() => {
        setSelectSelectedItems([]);
      })
      .catch((error) => {
        console.error('Delete channels Error: ', error.message);
        throw error;
      })
      .finally(() => {
        dialogRef.current?.close();
      });
  }, [selectSelectedItems, selectAll, ReturnToParent, queryFilter, setSelectSelectedItems, setSelectAll]);

  return (
    <SMDialog
      ref={dialogRef}
      onHide={ReturnToParent}
      buttonDisabled={getTotalCount === 0}
      widthSize={2}
      iconFilled
      title="DELETE CHANNELS"
      icon="pi-times"
      buttonClassName="icon-red"
    >
      <div className="text-base">
        Delete ({selectAll ? 'All' : getTotalCount}) channles?
        <div className="flex align-content-center justify-content-end">
          <OKButton onClick={onOkClick} />
        </div>
      </div>
    </SMDialog>
  );
};

DeleteSMChannelsDialog.displayName = 'DeleteSMChannelsDialog';

export default React.memo(DeleteSMChannelsDialog);
