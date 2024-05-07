import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';

import BaseButton from '@components/buttons/BaseButton';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { ToggleSMStreamVisibleByParameters, ToggleSMStreamsVisibleById } from '@lib/smAPI/SMStreams/SMStreamsCommands';
import { SMStreamDto, ToggleSMStreamVisibleByParametersRequest, ToggleSMStreamsVisibleByIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';

interface StreamMultiVisibleDialogProperties {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  readonly selectedItemsKey: string;
}

const StreamMultiVisibleDialog = ({ id, iconFilled, onClose, skipOverLayer, selectedItemsKey }: StreamMultiVisibleDialogProperties) => {
  const { selectSelectedItems } = useSelectedItems<SMStreamDto>(selectedItemsKey);

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const ReturnToParent = useCallback(() => {
    onClose?.();
  }, [onClose]);

  const getTotalCount = useMemo(() => selectSelectedItems?.length ?? 0, [selectSelectedItems]);

  const onVisiblesClick = useCallback(async () => {
    if (selectSelectedItems === undefined) {
      ReturnToParent();
      return;
    }

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();
        return;
      }

      const request = {} as ToggleSMStreamVisibleByParametersRequest;
      request.Parameters = queryFilter;

      await ToggleSMStreamVisibleByParameters(request)
        .then(() => {})
        .catch((error) => {
          console.error(error);
          throw error;
        });

      return;
    }

    if (selectSelectedItems.length === 0) {
      ReturnToParent();

      return;
    }

    const request = {} as ToggleSMStreamsVisibleByIdRequest;

    const ids = selectSelectedItems.map((item) => item.Id);
    request.Ids = ids;

    await ToggleSMStreamsVisibleById(request)
      .then(() => {})
      .catch((error) => {
        console.error('Set Stream Visibility Error: ', error.message);
        throw error;
      });
  }, [selectSelectedItems, selectAll, ReturnToParent, queryFilter]);

  return (
    <BaseButton
      className="icon-red-filled"
      disabled={getTotalCount === 0}
      icon="pi-eye-slash"
      rounded
      onClick={async (event) => {
        await onVisiblesClick();
      }}
      aria-controls="popup_menu_right"
      aria-haspopup
      tooltip="Toggle Visibility"
    />
  );
};

StreamMultiVisibleDialog.displayName = 'StreamMultiVisibleDialog';

export default memo(StreamMultiVisibleDialog);
