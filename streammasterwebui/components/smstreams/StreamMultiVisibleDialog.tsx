import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';

import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { ToggleSMStreamVisibleByParameters, ToggleSMStreamsVisibleById } from '@lib/smAPI/SMStreams/SMStreamsCommands';
import { SMStreamDto, ToggleSMStreamVisibleByParametersRequest, ToggleSMStreamsVisibleByIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';
import VisibleButton from '../buttons/VisibleButton';

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
    <div>
      <VisibleButton
        disabled={getTotalCount === 0}
        iconFilled={iconFilled}
        // label="Toggle Visibility"
        onClick={async () => await onVisiblesClick()}
        tooltip="Toggle Visibility"
        isLeft
      />
    </div>
  );
};

StreamMultiVisibleDialog.displayName = 'StreamMultiVisibleDialog';

export default memo(StreamMultiVisibleDialog);
