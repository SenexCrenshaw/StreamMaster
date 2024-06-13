import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMPopUp from '@components/sm/SMPopUp';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { UpdateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import SMChannelDialog, { SMChannelDialogRef } from './SMChannelDialog';

interface CopySMChannelProperties {
  smChannel: SMChannelDto;
}

const EditSMChannelDialog = ({ smChannel }: CopySMChannelProperties) => {
  const dialogRef = useRef<SMChannelDialogRef>(null);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const [originalDto, setOriginalDto] = useState<SMChannelDto | undefined>(undefined);
  const { isTrue: isNameLoading, setIsTrue: setIsNameLoading } = useIsTrue('SMChannelDataSelectorIsNameLoading');

  useEffect(() => {
    if (smChannel === undefined) {
      return;
    }
    setOriginalDto(smChannel);

    return;
  }, [smChannel]);

  const onSave = useCallback(
    (request: UpdateSMChannelRequest) => {
      request.Id = smChannel.Id;
      setIsNameLoading(true);

      UpdateSMChannel(request)
        .then(() => {})
        .catch((e) => {
          console.error(e);
        })
        .finally(() => {
          setIsNameLoading(false);
        });
    },
    [smChannel, setIsNameLoading]
  );

  if (smChannel === undefined) {
    return null;
  }

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      contentWidthSize="5"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            buttonDisabled={!saveEnabled && originalDto !== undefined}
            onClick={() => {
              dialogRef.current?.reset();
            }}
          />
          <OKButton
            buttonDisabled={!saveEnabled}
            onClick={(request) => {
              dialogRef.current?.save();
            }}
          />
        </div>
      }
      isLoading={isNameLoading}
      icon="pi-pencil"
      modal
      modalCentered
      placement="bottom-end"
      rememberKey={'DeleteChannelGroupDialog'}
      title={`EDIT CHANNEL : ${smChannel.Name}`}
    >
      <SMChannelDialog ref={dialogRef} smChannel={smChannel} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

EditSMChannelDialog.displayName = 'EditSMChannelDialog';

export default React.memo(EditSMChannelDialog);
