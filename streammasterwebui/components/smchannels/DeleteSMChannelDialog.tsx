import { SMPopUp } from '@components/sm/SMPopUp';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannelRequest, SMChannelDto, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useState } from 'react';

interface DeleteSMChannelProperties {
  readonly smChannel: SMChannelDto;
}

const DeleteSMChannelDialog = ({ smChannel }: DeleteSMChannelProperties) => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [isCalled, setIsCalled] = useState(false);

  const ReturnToParent = React.useCallback(() => {
    setSelectedItems([]);
    setIsCalled(false);
  }, [setSelectedItems]);

  const accept = React.useCallback(() => {
    if (isCalled) return;
    setIsCalled(true);

    const toSend = {} as DeleteSMChannelRequest;
    toSend.SMChannelId = smChannel.Id;
    DeleteSMChannel(toSend)
      .then((response) => {
        console.log('Delete Channel');
      })
      .catch((error) => {
        console.error('Delete Channel', error.message);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, isCalled, smChannel.Id]);

  return <SMPopUp rememberKey={'DeleteSMChannelDialog'} title="Delete" OK={() => accept()} icon="pi-times" />;
};

DeleteSMChannelDialog.displayName = 'DeleteSMChannelDialog';

export default React.memo(DeleteSMChannelDialog);
