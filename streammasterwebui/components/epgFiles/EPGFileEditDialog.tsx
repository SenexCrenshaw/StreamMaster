import { memo, useRef, useState } from 'react';

import ResetButton from '@components/buttons/ResetButton';
import SMPopUp from '@components/sm/SMPopUp';
import { EPGFileDto } from '@lib/smAPI/smapiTypes';
import EPGFileDialog, { EPGFileDialogRef } from './EPGFileDialog';

interface EPGFileEditDialogProperties {
  readonly selectedFile: EPGFileDto;
}

const EPGFileEditDialog = ({ selectedFile }: EPGFileEditDialogProperties) => {
  const epgDialogRef = useRef<EPGFileDialogRef>(null);

  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      contentWidthSize="3"
      icon="pi-pencil"
      iconFilled={false}
      modal
      okButtonDisabled={!saveEnabled}
      onOkClick={() => {
        epgDialogRef.current?.save();
      }}
      placement="bottom-end"
      title="EDIT EPG"
      tooltip="Edit EPG"
      zIndex={11}
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            buttonDisabled={!saveEnabled}
            onClick={() => {
              epgDialogRef.current?.reset();
            }}
          />
        </div>
      }
    >
      <div className="layout-padding-bottom-lg" />
      <EPGFileDialog ref={epgDialogRef} onSaveEnabled={setSaveEnabled} selectedFile={selectedFile} showUrlEditor />
    </SMPopUp>
  );
};

EPGFileEditDialog.displayName = 'EPGFileEditDialog';

export default memo(EPGFileEditDialog);
