import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

import OKButton from '@components/buttons/OKButton';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
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
  const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(selectedItemsKey);

  const { selectAll, setSelectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);
  const ReturnToParent = useCallback(() => {
    onClose?.();
  }, [onClose]);

  const onOkClick = useCallback(async () => {
    if (selectedItems === undefined) {
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
          setSelectedItems([]);
          setSelectAll(false);
        })
        .catch((error) => {
          console.error(error);
          throw error;
        })
        .finally(() => {
          dialogRef.current?.hide();
        });

      return;
    }

    if (selectedItems.length === 0) {
      ReturnToParent();

      return;
    }

    const request = {} as CreateSMChannelFromStreamsRequest;

    const ids = selectedItems.map((item) => item.Id);
    request.StreamIds = ids;

    await CreateSMChannelFromStreams(request)
      .then(() => {
        setSelectedItems([]);
      })
      .catch((error) => {
        console.error('Create channels Error: ', error.message);
        throw error;
      })
      .finally(() => {
        dialogRef.current?.hide();
      });
  }, [selectedItems, selectAll, ReturnToParent, queryFilter, setSelectedItems, setSelectAll]);

  return (
    <SMDialog
      ref={dialogRef}
      buttonDisabled={getTotalCount === 0}
      position="top-right"
      title="Bulk CREATE CHANNELS"
      iconFilled
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green"
      icon="pi-building-columns"
      widthSize={2}
      info="General"
      tooltip="Bulk Create Channels"
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
