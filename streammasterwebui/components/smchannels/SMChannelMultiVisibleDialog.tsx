import SMButton from '@components/sm/SMButton';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { ToggleSMChannelVisibleByParameters, ToggleSMChannelsVisibleById } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { SMChannelDto, ToggleSMChannelVisibleByParametersRequest, ToggleSMChannelsVisibleByIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';

interface SMChannelMultiVisibleDialogProperties {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  readonly selectedItemsKey: string;
}

const SMChannelMultiVisibleDialog = ({ id, iconFilled, onClose, skipOverLayer, selectedItemsKey }: SMChannelMultiVisibleDialogProperties) => {
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);

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

      const request = {} as ToggleSMChannelVisibleByParametersRequest;
      request.Parameters = queryFilter;

      await ToggleSMChannelVisibleByParameters(request)
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

    const request = {} as ToggleSMChannelsVisibleByIdRequest;

    const ids = selectedItems.map((item) => item.Id);
    request.Ids = ids;

    await ToggleSMChannelsVisibleById(request)
      .then(() => {})
      .catch((error) => {
        console.error('Set SMChannel Visibility Error: ', error.message);
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

SMChannelMultiVisibleDialog.displayName = 'SMChannelMultiVisibleDialog';

export default memo(SMChannelMultiVisibleDialog);
