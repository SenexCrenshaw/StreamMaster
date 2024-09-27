import OKButton from '@components/buttons/OKButton';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

import { CreateMultiViewChannelRequest, SMChannelDto, UpdateMultiViewChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useRef, useState } from 'react';
import { SMChannelDialogRef } from './SMChannelDialog';
import SMMultiViewChannelDialog from './SMMultiViewChannelDialog';
import { CreateMultiViewChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';

const CreateMultiViewSMChannelDialog = () => {
  const dataKey = 'CreateSMultiViewChannelDialog';
  const { selectedItems, setSelectedItems } = useSelectedItems<SMChannelDto>(dataKey);

  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const smDialogRef = useRef<SMChannelDialogRef>(null);
  const popUpRef = useRef<SMPopUpRef>(null);

  const ReturnToParent = React.useCallback(() => {
    popUpRef.current?.hide();
    setSelectedItems([]);
  }, [setSelectedItems]);

  const onSave = React.useCallback(
    (updateSMChannelRequest: UpdateMultiViewChannelRequest) => {
      const request = updateSMChannelRequest as CreateMultiViewChannelRequest;
      request.SMSChannelIds = selectedItems.map((e) => e.Id);

      CreateMultiViewChannel(request)
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
      buttonClassName="icon-orange"
      onCloseClick={ReturnToParent}
      contentWidthSize="5"
      icon="pi-microsoft"
      iconFilled
      modal
      modalCentered
      noBorderChildren
      title="MultiView CHANNEL"
      tooltip="Create MultiView Channel"
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
      <SMMultiViewChannelDialog dataKey={dataKey} ref={smDialogRef} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

CreateMultiViewSMChannelDialog.displayName = 'CreateMultiViewSMChannelDialog';

export default React.memo(CreateMultiViewSMChannelDialog);
