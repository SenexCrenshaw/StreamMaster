import SMButton from '@components/sm/SMButton';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { ToggleSMStreamVisibleByParameters, ToggleSMStreamsVisibleById } from '@lib/smAPI/SMStreams/SMStreamsCommands';
import { SMStreamDto, ToggleSMStreamVisibleByParametersRequest, ToggleSMStreamsVisibleByIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';

interface StreamMultiVisibleDialogProperties {
  readonly id: string;
  readonly onClose?: () => void;
  readonly selectedItemsKey: string;
}

const StreamMultiVisibleDialog = ({ id, onClose, selectedItemsKey }: StreamMultiVisibleDialogProperties) => {
  const { selectedItems } = useSelectedItems<SMStreamDto>(selectedItemsKey);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const ReturnToParent = useCallback(() => {
    onClose?.();
  }, [onClose]);

  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);

  const onVisiblesClick = useCallback(async () => {
    if (selectedItems === undefined) {
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

    if (selectedItems.length === 0) {
      ReturnToParent();

      return;
    }

    const request = {} as ToggleSMStreamsVisibleByIdRequest;

    const ids = selectedItems.map((item) => item.Id);
    request.Ids = ids;

    await ToggleSMStreamsVisibleById(request)
      .then(() => {})
      .catch((error) => {
        console.error('Set Stream Visibility Error: ', error.message);
        throw error;
      });
  }, [selectedItems, selectAll, ReturnToParent, queryFilter]);

  return (
    <SMButton
      className="icon-red"
      disabled={getTotalCount === 0}
      icon="pi-eye-slash"
      iconFilled
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
