import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { CreateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useRef, useState } from 'react';
import SMChannelDialog, { SMChannelDialogRef } from './SMChannelDialog';

const CreateSMChannelDialog = () => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const m3uDialogRef = useRef<SMChannelDialogRef>(null);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    setSelectedItems([]);
  }, [setSelectedItems]);

  const onSave = React.useCallback((request: CreateSMChannelRequest) => {
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
    <SMDialog
      darkBackGround
      ref={smDialogRef}
      position="top"
      title="CREATE CHANNEL"
      iconFilled
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green"
      icon="pi-plus"
      widthSize={5}
      tooltip="Create Channel"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <OKButton
            buttonDisabled={!saveEnabled}
            onClick={(request) => {
              m3uDialogRef.current?.save();
              smDialogRef.current?.hide();
            }}
          />
        </div>
      }
    >
      <SMChannelDialog ref={m3uDialogRef} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMDialog>
  );
};

CreateSMChannelDialog.displayName = 'CreateSMChannelDialog';

export default React.memo(CreateSMChannelDialog);
