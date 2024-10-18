import VisibleButton from '@components/buttons/VisibleButton';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { ToggleSMChannelVisibleByParameters, ToggleSMChannelsVisibleById } from '@lib/smAPI/SMChannels/SMChannelsCommands';

import { SMChannelDto, ToggleSMChannelVisibleByParametersRequest, ToggleSMChannelsVisibleByIdRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';

interface SMChannelMultiVisibleDialogProperties {
  readonly onClose?: () => void;
  readonly selectedItemsKey: string;
  readonly menu?: boolean;
}

const SMChannelMultiVisibleDialog = ({ menu, onClose, selectedItemsKey }: SMChannelMultiVisibleDialogProperties) => {
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');

  const ReturnToParent = useCallback(() => {
    onClose?.();
  }, [onClose]);

  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);

  const onVisiblesClick = useCallback(async () => {
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

    if ((selectedItems === undefined || selectedItems.length) === 0) {
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

  // if (menu === true) {
  //   return (
  //     <SMPopUp
  //       buttonClassName="icon-blue"
  //       icon="pi-eye-slash"
  //       iconFilled
  //       info=""
  //       label="Toggle Visibility"
  //       menu
  //       onOkClick={async () => await onVisiblesClick()}
  //       placement={smTableIsSimple ? 'bottom-end' : 'bottom'}
  //       title="Auto Set EPG"
  //       tooltip="Auto Set EPG"
  //     ></SMPopUp>
  //   );
  // }

  return (
    <VisibleButton
      buttonDisabled={getTotalCount === 0}
      iconFilled
      menu={menu}
      label={menu ? `Toggle (${selectAll ? 'All' : getTotalCount}) Visibility` : undefined}
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
