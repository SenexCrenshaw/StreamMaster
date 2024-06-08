import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { SMChannelDialogRef } from '@components/smchannels/SMChannelDialog';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { CreateSMStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import React, { useRef, useState } from 'react';
import SMStreamDialog from './SMStreamDialog';

const CreateSMStreamDialog = () => {
  const dataKey = 'SMChannelSMStreamDialog-SMStreamDataForSMChannelSelector';
  const { setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const m3uDialogRef = useRef<SMChannelDialogRef>(null);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {
    setSelectedItems([]);
  }, [setSelectedItems]);

  const onSave = React.useCallback((request: any) => {
    CreateSMStream(request)
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
      title="CREATE STREAM"
      iconFilled
      onHide={() => ReturnToParent()}
      buttonClassName="icon-green"
      icon="pi-plus"
      widthSize={5}
      info="General"
      tooltip="Create Stream"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <OKButton
            disabled={!saveEnabled}
            onClick={(request) => {
              m3uDialogRef.current?.save();
              smDialogRef.current?.hide();
            }}
          />
        </div>
      }
    >
      <SMStreamDialog ref={m3uDialogRef} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMDialog>
  );
};

CreateSMStreamDialog.displayName = 'CreateSMStreamDialog';

export default React.memo(CreateSMStreamDialog);
