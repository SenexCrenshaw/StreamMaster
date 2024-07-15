import OKButton from '@components/buttons/OKButton';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { CreateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { CreateSMChannelRequest, SMStreamDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useRef, useState } from 'react';
import SMChannelDialog, { SMChannelDialogRef } from './SMChannelDialog';

const CreateSMChannelDialog = () => {
  const dataKey = 'SMChannelSMStreamDialog';
  const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const smDialogRef = useRef<SMChannelDialogRef>(null);
  const popUpRef = useRef<SMPopUpRef>(null);

  const ReturnToParent = React.useCallback(() => {
    popUpRef.current?.hide();
    setSelectedItems([]);
  }, [setSelectedItems]);

  const onSave = React.useCallback(
    (updateSMChannelRequest: UpdateSMChannelRequest) => {
      const request = updateSMChannelRequest as CreateSMChannelRequest;
      request.SMStreamsIds = selectedItems.map((e) => e.Id);

      CreateSMChannel(request)
        .then(() => {})
        .catch((e: any) => {
          console.error(e);
        })
        .finally(() => {
          ReturnToParent();
        });
    },
    [ReturnToParent, selectedItems]
  );

  return (
    <SMPopUp
      ref={popUpRef}
      buttonClassName="icon-green"
      onCloseClick={ReturnToParent}
      contentWidthSize="5"
      icon="pi-plus"
      iconFilled
      modal
      modalCentered
      noBorderChildren
      title="CREATE CHANNEL"
      tooltip="Create Channel"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <OKButton
            buttonDisabled={!saveEnabled}
            onClick={(request) => {
              smDialogRef.current?.save();
            }}
          />
        </div>
      }
    >
      <SMChannelDialog ref={smDialogRef} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

CreateSMChannelDialog.displayName = 'CreateSMChannelDialog';

export default React.memo(CreateSMChannelDialog);
