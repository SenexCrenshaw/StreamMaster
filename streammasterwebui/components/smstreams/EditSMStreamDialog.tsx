import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { SMChannelDialogRef } from '@components/smchannels/SMChannelDialog';
import { UpdateSMStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMStreamDto, UpdateSMStreamRequest } from '@lib/smAPI/smapiTypes';
import React, { useRef, useState } from 'react';
import SMStreamDialog from './SMStreamDialog';

interface EditSMStreamDialogProperties {
  smStreamDto: SMStreamDto;
}

const EditSMStreamDialog = ({ smStreamDto }: EditSMStreamDialogProperties) => {
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const m3uDialogRef = useRef<SMChannelDialogRef>(null);
  const smDialogRef = useRef<SMDialogRef>(null);

  const ReturnToParent = React.useCallback(() => {}, []);

  const onSave = React.useCallback(
    (request: any) => {
      const r = request as UpdateSMStreamRequest;
      r.SMStreamId = smStreamDto.Id;

      UpdateSMStream(r)
        .then(() => {})
        .catch((e: any) => {
          console.error(e);
        })
        .finally(() => {
          smDialogRef.current?.hide();
        });
    },
    [smStreamDto.Id]
  );

  return (
    <SMDialog
      darkBackGround
      buttonDisabled={smStreamDto === undefined || smStreamDto.IsUserCreated === false}
      ref={smDialogRef}
      position="top-right"
      title="EDIT STREAM"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-yellow"
      icon="pi-pencil"
      widthSize={5}
      info="General"
      tooltip="Edit Stream"
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
      <SMStreamDialog
        ref={m3uDialogRef}
        onSave={onSave}
        onSaveEnabled={(e) => {
          setSaveEnabled(e);
          // Logger.debug('EditSMStreamDialog.onSaveEnabled', e);
        }}
        smStreamDto={smStreamDto}
      />
    </SMDialog>
  );
};

EditSMStreamDialog.displayName = 'EditSMStreamDialog';

export default React.memo(EditSMStreamDialog);
