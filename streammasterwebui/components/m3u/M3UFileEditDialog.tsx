import ResetButton from '@components/buttons/ResetButton';
import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { isNumber } from '@lib/common/common';
import { getEnumValueByKey } from '@lib/common/enumTools';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UField, M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useRef, useState } from 'react';
import M3UFileDialog, { M3UFileDialogRef } from './M3UFileDialog';

interface M3UFileEditDialogProperties {
  readonly selectedFile: M3UFileDto;
}

const M3UFileEditDialog = ({ selectedFile }: M3UFileEditDialogProperties) => {
  const m3uDialogRef = useRef<M3UFileDialogRef>(null);
  const [saveEnabled, setSaveEnabled] = useState<boolean>(false);
  const [request, setRequest] = useState<UpdateM3UFileRequest>({} as UpdateM3UFileRequest);
  const smPopupRef = useRef<SMPopUpRef>(null);

  const onSave = useCallback(async () => {
    if (Object.keys(request).length === 0) {
      return;
    }

    if (selectedFile?.Id === undefined) {
      return;
    }

    // Logger.debug('M3UFileDialog Id', selectedFile.Id);
    request.Id = selectedFile.Id;

    if (isNumber(request.M3UName)) {
      const a = getEnumValueByKey(M3UField, request.M3UName.toString() as keyof typeof M3UField);
      request.M3UName = a;
    }

    if (request.Id === undefined) {
      return;
    }
    try {
      await UpdateM3UFile(request);
    } catch (error) {
      console.error(error);
    } finally {
      m3uDialogRef.current?.reset();
      smPopupRef.current?.hide();
    }
  }, [request, selectedFile.Id]);

  if (selectedFile === undefined) {
    return null;
  }

  return (
    <SMPopUp
      buttonClassName="icon-yellow"
      contentWidthSize="5"
      onOkClick={onSave}
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
      noCloseButton={false}
      placement="bottom-end"
      ref={smPopupRef}
      title="EDIT M3U"
      zIndex={11}
    >
      <div className="layout-padding-bottom-lg" />
      <M3UFileDialog onRequestChanged={setRequest} ref={m3uDialogRef} showUrlEditor m3uFileDto={selectedFile} onSaveEnabled={setSaveEnabled} />
    </SMPopUp>
  );
};

M3UFileEditDialog.displayName = 'M3UFileEditDialog';

export default memo(M3UFileEditDialog);
