import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';

import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import React, { useRef } from 'react';

interface CopySMChannelProperties {
  readonly onHide?: () => void;
  smChannel: SMChannelDto;
}

const EditSMChannelDialog = ({ onHide, smChannel }: CopySMChannelProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);
  // const [request, setRequest] = React.useState<UpdateSMCHa>({} as UpdateM3UFileRequest);
  // const setName = useCallback(
  //   (value: string) => {
  //     if (smChannel && smChannel.Name !== value) {
  //       const epgFileDtoCopy = { ...smChannel };
  //       epgFileDtoCopy.Name = value;
  //       setEPGFileDto(epgFileDtoCopy);

  //       const requestCopy = { ...request };
  //       requestCopy.Id = epgFileDtoCopy.Id;
  //       requestCopy.Name = value;
  //       setRequest(requestCopy);

  //       onEPGChanged && onEPGChanged(epgFileDtoCopy);
  //     }
  //   },
  //   [epgFileDto, onEPGChanged, request]
  // );

  const ReturnToParent = React.useCallback(() => {
    onHide?.();
  }, [onHide]);

  const onSave = React.useCallback(async () => {}, []);

  return (
    <SMDialog
      ref={smDialogRef}
      iconFilled={false}
      title={`EDIT CHANNEL : ${smChannel.Name}`}
      onHide={() => ReturnToParent()}
      buttonClassName="icon-yellow"
      icon="pi-pencil"
      widthSize={2}
      info="General"
      tooltip="Edit Channel"
    >
      <div className="w-12">
        <div className="surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className="flex col-12 pl-1 justify-content-start align-items-center p-0 m-0 w-full">
            {/* <StringEditor label="Name" darkBackGround disableDebounce onChange={(e) => e && console.log(e)} onSave={(e) => {}} value={newName} /> */}
          </div>
          <div className="flex col-12 gap-2 mt-4 justify-content-center ">
            <OKButton onClick={async () => await onSave()} />
          </div>
        </div>
        <div className="layout-padding-bottom-lg" />
      </div>
    </SMDialog>
  );
};

EditSMChannelDialog.displayName = 'EditSMChannelDialog';

export default React.memo(EditSMChannelDialog);
