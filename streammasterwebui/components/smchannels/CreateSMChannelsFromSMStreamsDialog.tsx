import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

import SMPopUp from '@components/sm/SMPopUp';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { CreateSMChannelFromStreamParameters, CreateSMChannelFromStreams } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelFromStreamParametersRequest, CreateSMChannelFromStreamsRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useMemo } from 'react';

interface CreateSMChannelsDialogProperties {
  readonly onClose?: () => void;
  readonly id: string;
  readonly label?: string;
  readonly selectedItemsKey: string;
}

const CreateSMChannelsFromSMStreamsDialog = ({ id, label, onClose, selectedItemsKey }: CreateSMChannelsDialogProperties) => {
  const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(selectedItemsKey);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
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
      if (selectedStreamGroup?.Id) {
        request.StreamGroupId = selectedStreamGroup.Id;
      }

      await CreateSMChannelFromStreamParameters(request)
        .then(() => {
          setSelectedItems([]);
          setSelectAll(false);
        })
        .catch((error) => {
          console.error(error);
          throw error;
        })
        .finally(() => {});

      return;
    }

    if (selectedItems.length === 0) {
      ReturnToParent();

      return;
    }

    const request = {} as CreateSMChannelFromStreamsRequest;

    const ids = selectedItems.map((item) => item.Id);
    request.StreamIds = ids;
    if (selectedStreamGroup?.Id) {
      request.StreamGroupId = selectedStreamGroup.Id;
    }

    await CreateSMChannelFromStreams(request)
      .then(() => {
        setSelectedItems([]);
      })
      .catch((error) => {
        console.error('Create channels Error: ', error.message);
        throw error;
      })
      .finally(() => {});
  }, [selectedItems, selectAll, selectedStreamGroup, ReturnToParent, queryFilter, setSelectedItems, setSelectAll]);

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      buttonDisabled={getTotalCount < 1}
      buttonLabel="Add Selected"
      icon="pi-plus-circle"
      iconFilled
      info=""
      menu
      onOkClick={() => onOkClick()}
      placement="bottom-end"
      showRemember={false}
      title="Create"
    >
      <div className="sm-center-stuff">
        <div className="text-container">Create ({selectAll ? 'All' : getTotalCount}) channels?</div>
      </div>
    </SMPopUp>
  );
};

CreateSMChannelsFromSMStreamsDialog.displayName = 'CreateSMChannelsDialog';

export default React.memo(CreateSMChannelsFromSMStreamsDialog);
