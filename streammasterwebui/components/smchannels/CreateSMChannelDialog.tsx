import OKButton from '@components/buttons/OKButton';
import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp from '@components/sm/SMPopUp';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { CreateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelRequest, SMStreamDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useRef, useState } from 'react';
import SMChannelDialog, { SMChannelDialogRef } from './SMChannelDialog';

const CreateSMChannelDialog = () => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const dialogRef = useRef<SMChannelDialogRef>(null);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    setSelectedItems([]);
  }, [setSelectedItems]);

  const onSave = React.useCallback((updateSMChannelRequest: UpdateSMChannelRequest) => {
    const request = updateSMChannelRequest as CreateSMChannelRequest;

    CreateSMChannel(request)
      .then(() => {})
      .catch((e: any) => {
        console.error(e);
      })
      .finally(() => {
        smDialogRef.current?.hide();
      });
  }, []);

  return (
    <SMPopUp
      darkBackGround
      title="CREATE CHANNEL"
      iconFilled
      buttonClassName="icon-green"
      icon="pi-plus"
      contentWidthSize="5"
      tooltip="Create Channel"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <OKButton
            buttonDisabled={!saveEnabled}
            onClick={(request) => {
              dialogRef.current?.save();
            }}
          />
        </div>
      }
    >
      <SMChannelDialog ref={dialogRef} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

CreateSMChannelDialog.displayName = 'CreateSMChannelDialog';

export default React.memo(CreateSMChannelDialog);
