import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMPopUp from '@components/sm/SMPopUp';
import { UpdateSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetSMChannel from '@lib/smAPI/SMChannels/useGetSMChannel';
import { GetSMChannelRequest, SMChannelDto, UpdateSMChannelRequest } from '@lib/smAPI/smapiTypes';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import SMChannelDialog, { SMChannelDialogRef } from './SMChannelDialog';

interface CopySMChannelProperties {
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const EditSMChannelDialog = ({ onHide, smChannel }: CopySMChannelProperties) => {
  const dialogRef = useRef<SMChannelDialogRef>(null);
  const query = useGetSMChannel({ SMChannelId: smChannel.Id } as GetSMChannelRequest);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const [originalDto, setOriginalDto] = useState<SMChannelDto | undefined>(undefined);

  useEffect(() => {
    if (smChannel === undefined) {
      return;
    }

    // setM3UFileDto(selectedFile);
    setOriginalDto(smChannel);

    return;
  }, [smChannel]);

  // const ReturnToParent = React.useCallback(() => {
  //   onHide?.();
  // }, [onHide]);

  const onSave = useCallback(
    (request: UpdateSMChannelRequest) => {
      request.Id = smChannel.Id;

      UpdateSMChannel(request)
        .then(() => {})
        .catch((e) => {
          console.error(e);
        })
        .finally(() => {
          // dialogRef.current?.hide();
        });
    },
    [smChannel]
  );

  if (!query.data || smChannel === undefined) {
    return null;
  }

  return (
    <SMPopUp
      contentWidthSize="5"
      buttonClassName="icon-yellow"
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
              // smDialogRef.current?.hide();
            }}
          />
        </div>
      }
      modal
      modalCentered
      placement="bottom-end"
      rememberKey={'DeleteChannelGroupDialog'}
      title={`EDIT CHANNEL : ${smChannel.Name}`}
      icon="pi-pencil"
    >
      <SMChannelDialog ref={dialogRef} smChannel={query.data} onSave={onSave} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>

    // <SMDialog
    //   darkBackGround
    //   ref={smDialogRef}
    //   iconFilled={false}
    //   title={`EDIT CHANNEL : ${toGet.Name}`}
    //   onHide={() => ReturnToParent()}
    //   buttonClassName="icon-yellow"
    //   icon="pi-pencil"
    //   widthSize={5}
    //   tooltip="Edit Channel"
    // >
    //   <SMChannelDialog smChannel={query.data} onSave={onSave} />
    // </SMDialog>
  );
};

EditSMChannelDialog.displayName = 'EditSMChannelDialog';

export default React.memo(EditSMChannelDialog);
