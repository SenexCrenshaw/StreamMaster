import ResetButton from '@components/buttons/ResetButton';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import useIsRowLoading from '@lib/redux/hooks/useIsRowLoading';
import { UpdateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { lazy, Suspense, useCallback, useEffect, useRef, useState } from 'react';
import { SMChannelDialogRef } from './SMChannelDialog';

const SMChannelDialog = lazy(() => import('./SMChannelDialog'));

interface EditSMChannelDialogProperties {
  smChannelDto: SMChannelDto;
}

const EditSMChannelDialog = ({ smChannelDto }: EditSMChannelDialogProperties) => {
  const dataKey = 'EditSMChannelDialog';
  const dialogRef = useRef<SMChannelDialogRef>(null);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const [originalDto, setOriginalDto] = useState<SMChannelDto | undefined>(undefined);
  const propUpRef = useRef<SMPopUpRef>(null);

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
          propUpRef.current?.hide();
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
      contentWidthSize="6"
      icon="pi-pencil"
      info=""
      isPopupLoading={isRowLoading}
      modal
      modalCentered
      noBorderChildren
      noCloseButton={false}
      okButtonDisabled={!saveEnabled}
      ref={propUpRef}
      showRemember={false}
      title={`EDIT CHANNEL : ${smChannelDto.Name}`}
      onOkClick={() => {
        dialogRef.current?.save();
      }}
      header={
        <ResetButton
          buttonDisabled={!saveEnabled && originalDto !== undefined}
          onClick={() => {
            dialogRef.current?.reset();
          }}
        />
      }
    >
      <Suspense>
        <SMChannelDialog dataKey={dataKey} ref={dialogRef} smChannel={smChannelDto} onSave={onSave} onSaveEnabled={setSaveEnabled} />
      </Suspense>
    </SMPopUp>
  );
};

EditSMChannelDialog.displayName = 'EditSMChannelDialog';

export default React.memo(EditSMChannelDialog);
