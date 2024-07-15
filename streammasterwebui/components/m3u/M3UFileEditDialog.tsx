import ResetButton from '@components/buttons/ResetButton';
import SMPopUp from '@components/sm/SMPopUp';
import { M3UFileDto } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useRef, useState } from 'react';
import M3UFileDialog, { M3UFileDialogRef } from './M3UFileDialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const m3uDialogRef = useRef<M3UFileDialogRef>(null);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);

  const onSave = useCallback(() => {
    if (m3uDialogRef.current) {
      m3uDialogRef.current.save();
    }
  }, []);

  if (selectedFile === undefined) {
    return null;
  }

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      contentWidthSize="3"
      header={
        <div className="flex w-12 gap-1 justify-content-end align-content-center">
          <ResetButton
            buttonDisabled={!saveEnabled}
            onClick={() => {
              m3uDialogRef.current?.reset();
            }}
          />
        </div>
      }
      icon="pi-pencil"
      modal
      okButtonDisabled={!saveEnabled}
      onOkClick={onSave}
      noCloseButton={false}
      placement="bottom-end"
      showRemember={false}
      title="EDIT M3U"
      zIndex={11}
    >
      <div className="layout-padding-bottom-lg" />
      <M3UFileDialog ref={m3uDialogRef} showUrlEditor m3uFileDto={selectedFile} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
