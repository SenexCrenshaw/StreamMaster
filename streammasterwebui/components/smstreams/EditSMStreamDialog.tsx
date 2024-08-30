import ResetButton from '@components/buttons/ResetButton';
import SMPopUp from '@components/sm/SMPopUp';
import { SMChannelDialogRef } from '@components/smchannels/SMChannelDialog';
import useIsRowLoading from '@lib/redux/hooks/useIsRowLoading';
import { UpdateSMStream } from '@lib/smAPI/SMStreams/SMStreamsCommands';
import { SMStreamDto, UpdateSMStreamRequest } from '@lib/smAPI/smapiTypes';
import React, { useEffect, useRef, useState } from 'react';
import SMStreamDialog from './SMStreamDialog';

interface EditSMStreamDialogProperties {
  smStreamDto: SMStreamDto;
}

const EditSMStreamDialog = ({ smStreamDto }: EditSMStreamDialogProperties) => {
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const dialogRef = useRef<SMChannelDialogRef>(null);
  const [originalDto, setOriginalDto] = useState<SMStreamDto | undefined>(undefined);
  const [isRowLoading, setIsRowLoading] = useIsRowLoading({ Entity: 'SMStream', Id: smStreamDto.Id.toString() });

  useEffect(() => {
    if (smStreamDto === undefined) {
      return;
    }
    setOriginalDto(smStreamDto);

    return;
  }, [smStreamDto]);

  const onSave = React.useCallback(
    (request: any) => {
      setIsRowLoading(true);
      const r = request as UpdateSMStreamRequest;
      r.SMStreamId = smStreamDto.Id;

      UpdateSMStream(r)
        .then(() => {})
        .catch((e: any) => {
          console.error(e);
        })
        .finally(() => {
          setIsRowLoading(false);
        });
    },
    [setIsRowLoading, smStreamDto.Id]
  );

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      buttonDisabled={smStreamDto === undefined || smStreamDto.IsUserCreated !== true}
      contentWidthSize="5"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            buttonDisabled={!saveEnabled && originalDto !== undefined}
            onClick={() => {
              dialogRef.current?.reset();
            }}
          />
        </div>
      }
      icon="pi-pencil"
      isPopupLoading={isRowLoading}
      modal
      noBorderChildren
      okButtonDisabled={!saveEnabled}
      okToolTip="Save"
      onOkClick={() => dialogRef.current?.save()}
      placement="bottom-end"
      title="Edit Stream"
      zIndex={10}
    >
      <SMStreamDialog
        ref={dialogRef}
        onSave={onSave}
        onSaveEnabled={(e) => {
          setSaveEnabled(e);
        }}
        smStreamDto={smStreamDto}
      />
    </SMPopUp>
  );
};

EditSMStreamDialog.displayName = 'EditSMStreamDialog';

export default React.memo(EditSMStreamDialog);
