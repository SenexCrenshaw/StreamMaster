import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMPopUp from '@components/sm/SMPopUp';
import useIsRowLoading from '@lib/redux/hooks/useIsRowLoading';
import { UpdateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import SMChannelDialog, { SMChannelDialogRef } from './SMChannelDialog';

interface CopySMChannelProperties {
  smChannelDto: SMChannelDto;
}

const EditSMChannelDialog = ({ smChannelDto }: CopySMChannelProperties) => {
  const dialogRef = useRef<SMChannelDialogRef>(null);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const [originalDto, setOriginalDto] = useState<SMChannelDto | undefined>(undefined);

  const [isRowLoading, setIsRowLoading] = useIsRowLoading({ Entity: 'SMChannel', Id: smChannelDto.Id.toString() });

  useEffect(() => {
    if (smChannelDto === undefined) {
      return;
    }
    setOriginalDto(smChannelDto);

    return;
  }, [smChannelDto]);

  const onSave = useCallback(
    (request: UpdateSMChannelRequest) => {
      request.Id = smChannelDto.Id;
      setIsRowLoading(true);

      UpdateSMChannel(request)
        .then(() => {})
        .catch((e) => {
          console.error(e);
        })
        .finally(() => {
          setIsRowLoading(false);
        });
    },
    [smChannelDto, setIsRowLoading]
  );

  // Logger.debug('EditSMChannelDialog', isRowLoading);

  if (smChannelDto === undefined) {
    return null;
  }

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      buttonIsLoading={isRowLoading}
      contentWidthSize="5"
      hasCloseButton={false}
      icon="pi-pencil"
      isPopupLoading={isRowLoading}
      modal
      modalCentered
      noBorderChildren
      simpleChildren
      showRemember={false}
      title={`EDIT CHANNEL : ${smChannelDto.Name}`}
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
    >
      <SMChannelDialog ref={dialogRef} smChannel={smChannelDto} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

EditSMChannelDialog.displayName = 'EditSMChannelDialog';

export default React.memo(EditSMChannelDialog);
