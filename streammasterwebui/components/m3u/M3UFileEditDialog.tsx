import OKButton from '@components/buttons/OKButton';
import ResetButton from '@components/buttons/ResetButton';
import SMPopUp from '@components/sm/SMPopUp';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { memo, useRef, useState } from 'react';
import M3UFileDialog, { M3UFileDialogRef } from './M3UFileDialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const m3uDialogRef = useRef<M3UFileDialogRef>(null);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);

  if (selectedFile === undefined) {
    return null;
  }

  return (
    <SMPopUp
      zIndex={11}
      buttonClassName="icon-yellow"
      contentWidthSize="3"
      hasCloseButton={false}
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            buttonDisabled={!saveEnabled}
            onClick={() => {
              m3uDialogRef.current?.reset();
            }}
          />
          <OKButton
            buttonDisabled={!saveEnabled}
            onClick={(request) => {
              m3uDialogRef.current?.save();
            }}
          />
        </div>
      }
      icon="pi-pencil"
      modal
      placement="bottom-end"
      showRemember={false}
      title="EDIT M3U"
    >
      <div className="layout-padding-bottom-lg" />
      <M3UFileDialog ref={m3uDialogRef} showUrlEditor m3uFileDto={selectedFile} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
