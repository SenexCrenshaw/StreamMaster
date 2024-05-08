import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { CreateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useRef } from 'react';
import SMChannelDialog from './SMChannelDialog';

// interface CreateSMChannelDialogProperties {}

const CreateSMChannelDialog = () => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { setSelectSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);

  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    setSelectSelectedItems([]);
  }, [setSelectSelectedItems]);

  const onSave = React.useCallback((request: CreateSMChannelRequest) => {
    CreateSMChannel(request)
      .then(() => {})
      .catch((e: any) => {
        console.error(e);
      })
      .finally(() => {
        smDialogRef.current?.close();
      });
  }, []);

  return (
    <SMDialog
      ref={smDialogRef}
      position="top"
      title="CREATE CHANNEL"
      iconFilled
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green"
      icon="pi-plus"
      widthSize={8}
      info="General"
      tooltip="Create Channel"
    >
      <SMChannelDialog onSave={onSave} />;
    </SMDialog>
  );
};

CreateSMChannelDialog.displayName = 'CreateSMChannelDialog';

export default React.memo(CreateSMChannelDialog);
