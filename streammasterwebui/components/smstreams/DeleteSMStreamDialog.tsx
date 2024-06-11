import { SMPopUp } from '@components/sm/SMPopUp';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { DeleteSMStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMStreamRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';

import React, { useState } from 'react';

interface DeleteSMStreamProperties {
  readonly smStream: SMStreamDto;
}

const DeleteSMStreamDialog = ({ smStream }: DeleteSMStreamProperties) => {
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

    const toSend = {} as DeleteSMStreamRequest;
    toSend.SMStreamId = smStream.Id;
    DeleteSMStream(toSend)
      .then((response) => {
        console.log('Delete Stream');
      })
      .catch((error) => {
        console.error('Delete Stream', error.message);
      })
      .finally(() => {
        ReturnToParent();
      });
  }, [ReturnToParent, isCalled, smStream.Id]);

  return (
    <SMPopUp
      buttonClassName="icon-red"
      buttonDisabled={smStream === undefined || !smStream.IsUserCreated}
      rememberKey={'DeleteSMStreamDialog'}
      title="Delete"
      onOkClick={() => accept()}
      icon="pi-times"
    />
  );
};

DeleteSMStreamDialog.displayName = 'DeleteSMStreamDialog';

export default React.memo(DeleteSMStreamDialog);
