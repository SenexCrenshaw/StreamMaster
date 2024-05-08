import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';

import OKButton from '@components/buttons/OKButton';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { CreateSMChannelFromStreamParameters, CreateSMChannelFromStreams } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelFromStreamParametersRequest, CreateSMChannelFromStreamsRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useMemo } from 'react';

interface CreateSMChannelsDialogProperties {
  readonly onClose?: () => void;
  readonly id: string;
  readonly selectedItemsKey: string;
}

const CreateSMChannelsDialog = ({ id, onClose, selectedItemsKey }: CreateSMChannelsDialogProperties) => {
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

      const request = {} as CreateSMChannelFromStreamParametersRequest;
      request.Parameters = queryFilter;

      await CreateSMChannelFromStreamParameters(request)
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

    const request = {} as CreateSMChannelFromStreamsRequest;

    const ids = selectSelectedItems.map((item) => item.Id);
    request.StreamIds = ids;

    await CreateSMChannelFromStreams(request)
      .then(() => {
        setSelectSelectedItems([]);
      })
      .catch((error) => {
        console.error('Create channels Error: ', error.message);
        throw error;
      })
      .finally(() => {
        dialogRef.current?.close();
      });
  }, [selectSelectedItems, selectAll, ReturnToParent, queryFilter, setSelectSelectedItems, setSelectAll]);

  return (
    <SMDialog
      ref={dialogRef}
      buttonDisabled={getTotalCount === 0}
      position="top-right"
      title="CREATE CHANNELS"
      iconFilled
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green"
      icon="pi-plus"
      widthSize={2}
      info="General"
      tooltip="Create Channels"
    >
      <div className="text-base">
        Create ({selectAll ? 'All' : getTotalCount}) channels?
        <div className="flex align-content-center justify-content-end">
          <OKButton onClick={onOkClick} />
        </div>
      </div>
    </SMDialog>
  );
};

CreateSMChannelsDialog.displayName = 'CreateSMChannelsDialog';

export default React.memo(CreateSMChannelsDialog);
