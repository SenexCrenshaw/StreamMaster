import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';

import { SMPopUp } from '@components/sm/SMPopUp';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannelRequest, SMChannelDto, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React from 'react';

interface DeleteSMChannelProperties {
  readonly smChannel: SMChannelDto;
}

const DeleteSMChannelDialog = ({ smChannel }: DeleteSMChannelProperties) => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { setSelectSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);

  const ReturnToParent = React.useCallback(() => {
    setSelectSelectedItems([]);
  }, [setSelectSelectedItems]);

  const accept = React.useCallback(() => {
    const toSend = {} as DeleteSMChannelRequest;
    toSend.SMChannelId = smChannel.Id;
    DeleteSMChannel(toSend)
      .then((response) => {
        console.log('Removed Channel');
      })
      .catch((error) => {
        console.error('Remove Channel', error.message);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, smChannel.Id]);

  return (
    <SMPopUp title="Remove Channel" OK={() => accept()} icon="pi-times" severity="danger">
      <div className="text-base">
        "{smChannel.Name}"
        <br />
        Are you sure?
      </div>
    </SMPopUp>
  );
};

DeleteSMChannelDialog.displayName = 'DeleteSMChannelDialog';

export default React.memo(DeleteSMChannelDialog);
