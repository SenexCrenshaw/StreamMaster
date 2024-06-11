import SMPopUp from '@components/sm/SMPopUp';
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
  const smChannelDialogRef = useRef<SMChannelDialogRef>(null);

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
          // smChannelDialogRef.current?.hide();
        });
    },
    [smStreamDto.Id]
  );

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      contentWidthSize="5"
      icon="pi-pencil"
      modal
      okButtonDisabled={!saveEnabled}
      onOkClick={() => smChannelDialogRef.current?.save()}
      placement="bottom-end"
      showRemember={false}
      title="Edit Stream"
      zIndex={10}
    >
      <SMStreamDialog
        ref={smChannelDialogRef}
        onSave={onSave}
        onSaveEnabled={(e) => {
          setSaveEnabled(e);
        }}
        smStreamDto={smStreamDto}
      />
    </SMPopUp>

    // <SMDialog
    //   darkBackGround
    //   buttonDisabled={smStreamDto === undefined || smStreamDto.IsUserCreated === false}
    //   ref={smDialogRef}
    //   position="top-right"
    //   title="EDIT STREAM"
    //   onHide={() => ReturnToParent()}
    //   buttonClassName="icon-yellow"
    //   icon="pi-pencil"
    //   widthSize={5}
    //   tooltip="Edit Stream"
    //   header={
    //     <div className="flex w-12 gap-1 justify-content-end align-content-center">
    //       <OKButton
    //         buttonDisabled={!saveEnabled}
    //         onClick={(request) => {
    //           m3uDialogRef.current?.save();
    //           smDialogRef.current?.hide();
    //         }}
    //       />
    //     </div>
    //   }
    // >
    //   <SMStreamDialog
    //     ref={m3uDialogRef}
    //     onSave={onSave}
    //     onSaveEnabled={(e) => {
    //       setSaveEnabled(e);
    //       // Logger.debug('EditSMStreamDialog.onSaveEnabled', e);
    //     }}
    //     smStreamDto={smStreamDto}
    //   />
    // </SMDialog>
  );
};

EditSMStreamDialog.displayName = 'EditSMStreamDialog';

export default React.memo(EditSMStreamDialog);
